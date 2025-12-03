using Microsoft.EntityFrameworkCore;
using OkPedidos.Core.Result;
using OkPedidos.Shared.Constants.Models;
using System.Globalization;
using System.Linq.Expressions;
using static OkPedidosAPI.Helpers.Enums;
using CoreDbFunctions = OkPedidos.Core.Data.DbFunctions;

namespace OkPedidos.Core.Services.Base
{
    public static class FilterService
    {
        private const string OPERATOR_EQ = "eq";
        private const string OPERATOR_NE = "ne";
        private const string OPERATOR_GT = "gt";
        private const string OPERATOR_LTE = "lte";
        private const string OPERATOR_GTE = "gte";
        private const string OPERATOR_LT = "lt";
        private const string OPERATOR_CO = "co";
        private const string OPERATOR_NCO = "nco";
        private const string OPERATOR_BE = "be";
        private const string OPERATOR_NBE = "nbe";

        private static Expression<Func<T, bool>> BuildExpression<T>(string propertyName, ComparisonOperator comparisonOperator, object value)
        {
            const string name = "x";
            var parameter = Expression.Parameter(typeof(T), name);
            var member = Expression.Property(parameter, propertyName);

            Expression body = comparisonOperator switch
            {
                ComparisonOperator.Contains => BuildContainsExpression(member, value), //co
                ComparisonOperator.NotContains => BuildNotContainsExpression(member, value), //nco
                ComparisonOperator.Between => BuildBetweenExpression(member, (Tuple<object, object>)value), //be
                ComparisonOperator.NotBetween => BuildNotBetweenExpression(member, (Tuple<object, object>)value), //nbe
                ComparisonOperator.Equal => BuildEqualExpression(member, value), //eq
                ComparisonOperator.NotEqual => BuildNotEqualExpression(member, value), //neq
                ComparisonOperator.GreaterThan => BuildGreaterThanExpression(member, value), //gt
                ComparisonOperator.GreaterThanOrEqual => BuildGreaterThanOrEqualExpression(member, value), //gte
                ComparisonOperator.LessThan => BuildLessThanExpression(member, value), //lt
                ComparisonOperator.LessThanOrEqual => BuildLessThanOrEqualExpression(member, value), //lte
                _ => throw new ArgumentException($"Invalid operator: {comparisonOperator}")
            };

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        private static Dictionary<string, (ComparisonOperator, object)> ParseFilters(Dictionary<string, string> filters)
        {
            var parsedFilters = new Dictionary<string, (ComparisonOperator, object)>();

            foreach (var filter in filters)
            {
                var parts = filter.Key.Split($"__");
                if (parts.Length == 2)
                {
                    var fieldName = parts[0];
                    var operatorString = parts[1];
                    var comparisonOperator = ParseOperator(operatorString);
                    var value = ParseValue(comparisonOperator, filter.Value);

                    parsedFilters.Add(fieldName, (comparisonOperator, value));
                }
            }

            return parsedFilters;
        }
        private static ComparisonOperator ParseOperator(string operatorString)
        {
            return operatorString switch
            {
                OPERATOR_EQ => ComparisonOperator.Equal,
                OPERATOR_NE => ComparisonOperator.NotEqual,
                OPERATOR_GT => ComparisonOperator.GreaterThan,
                OPERATOR_GTE => ComparisonOperator.GreaterThanOrEqual,
                OPERATOR_LT => ComparisonOperator.LessThan,
                OPERATOR_LTE => ComparisonOperator.LessThanOrEqual,
                OPERATOR_CO => ComparisonOperator.Contains,
                OPERATOR_NCO => ComparisonOperator.NotContains,
                OPERATOR_BE => ComparisonOperator.Between,
                OPERATOR_NBE => ComparisonOperator.NotBetween,
                _ => throw new ArgumentException($"Invalid operator: {operatorString}, valid values are [eq,ne,gt,gte,lt,lte,co,nco,be,nbe]")
            };
        }
        private static object ParseValue(ComparisonOperator comparisonOperator, string value)
        {
            return comparisonOperator switch
            {
                ComparisonOperator.Between or
                ComparisonOperator.NotBetween => ParseBetweenValues(value),
                _ => value
            };
        }
        private static (object startValue, object endValue) ParseBetweenValues(string value)
        {
            var parts = value.Split(',');

            if (parts.Length != 2)
            {
                throw new ArgumentException(ErrorMessage.InvalidFormatForBetweenOperator.Message);
            }

            var firstValue = parts[0].Trim();
            var secondValue = parts[1].Trim();


            if (DateTime.TryParse(firstValue, CultureInfo.CurrentCulture, out DateTime startDate) && DateTime.TryParse(secondValue, CultureInfo.CurrentCulture, out DateTime endDate))
            {
                return (startDate, endDate);
            }

            if (int.TryParse(firstValue, out int startNumber) && int.TryParse(secondValue, out int endNumber))
            {
                return (startNumber, endNumber);
            }

            if (double.TryParse(firstValue, out double startDouble) && double.TryParse(secondValue, out double endDouble))
            {
                return (startDouble, endDouble);
            }

            throw new ArgumentException(ErrorMessage.InvalidFormatForBetweenOperator.Message);
        }
        private static Expression BuildNotContainsExpression(MemberExpression member, object value)
        {
            return BuildContainsExpression(member, value, true);
        }
        private static Expression BuildContainsExpression(MemberExpression member, object value)
        {
            return BuildContainsExpression(member, value, false);
        }
        private static Expression BuildContainsExpression(MemberExpression member, object value, bool negate)
        {


            var propertyType = member.Type;
            if (propertyType == typeof(string))
            {

                var unaccentMethod = typeof(CoreDbFunctions).GetMethod(nameof(CoreDbFunctions.UnaccentOkPedidos), [typeof(string)]);
                var ilikeMethod = typeof(NpgsqlDbFunctionsExtensions).GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike), [typeof(DbFunctions), typeof(string), typeof(string)]);
                var dbFunctions = Expression.Property(null, typeof(EF).GetProperty(nameof(EF.Functions)));
                var unaccentExpression = Expression.Call(null, unaccentMethod, member);
                var ilikeExpression = Expression.Call(null, ilikeMethod, dbFunctions, unaccentExpression, Expression.Constant($"%{value}%"));
                return ilikeExpression;


            }
            else
            {
                const string msgArgumentException = "Value cannot be null or empty for 'contains' operator.";
                const string nameMethodContains = "Contains";
                const string nameMethodToString = "ToString";
                var stringValue = (value?.ToString()) ?? throw new ArgumentException(msgArgumentException);
                var containsMethod = typeof(string).GetMethod(nameMethodContains, [typeof(string)]);
                var containsExpression = Expression.Call(Expression.Call(member, typeof(object).GetMethod(nameMethodToString)), containsMethod, Expression.Constant(stringValue));
                return negate ? Expression.Not(containsExpression) : containsExpression;

            }



        }
        private static Expression BuildEqualExpression(MemberExpression member, object value)
        {
            const string TOLOWER = "ToLower";
            const string UNDEFINED_COLLECTION = "Cannot determine the type of the collection items.";
            const string CONTAINS = "Contains";

            if (value == null)
                return Expression.Constant(null);

            if (value is Expression expressionValue)
                return expressionValue;

            if (member.Type == typeof(string))
            {
                var toLowerMethod = typeof(string).GetMethod(TOLOWER, Type.EmptyTypes);

                var memberToLower = Expression.Call(member, toLowerMethod);
                var constantToLower = Expression.Constant(value.ToString().ToLower());

                return Expression.Equal(memberToLower, constantToLower);
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(member.Type)
                && member.Type != typeof(string))
            {
                var itemType = (member.Type.IsArray
                    ? member.Type.GetElementType()
                    : member.Type.GetGenericArguments().FirstOrDefault()) ?? throw new InvalidOperationException(UNDEFINED_COLLECTION);

                var typedValue = Convert.ChangeType(value, itemType);

                var constant = Expression.Constant(typedValue, itemType);

                var containsMethod = typeof(Enumerable)
                    .GetMethods()
                    .First(m => m.Name == CONTAINS && m.GetParameters().Length == 2)
                    .MakeGenericMethod(itemType);

                return Expression.Call(containsMethod, member, constant);
            }

            if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableType = member.Type.GetGenericArguments()[0];
                var constantExpression = Expression.Constant(Convert.ChangeType(value, nullableType), member.Type);
                return Expression.Equal(member, constantExpression);
            }
            else
            {
                const string msgArgumentException = $"The value provided for the enum must be an integer.";

                object convertedValue;
                if (member.Type.IsEnum)
                {
                    if (int.TryParse(value.ToString(), out int intValue))
                    {
                        convertedValue = Enum.ToObject(member.Type, intValue);
                    }
                    else
                    {
                        throw new ArgumentException(msgArgumentException, member.Type.Name);
                    }
                }
                else
                {
                    convertedValue = Convert.ChangeType(value, member.Type);
                }
                var constantExpression = Expression.Constant(convertedValue);

                return Expression.Equal(member, constantExpression);
            }

        }
        private static Expression BuildNotEqualExpression(MemberExpression member, object value)
        {

            if (value == null)
            {
                return Expression.Constant(null);
            }

            if (value is Expression expressionValue)
            {
                return expressionValue;
            }

            if (member.Type.IsEnum && Enum.TryParse(member.Type, value.ToString(), true, out object enumValue))
                value = enumValue;

            var constantExpression = Expression.Constant(Convert.ChangeType(value, member.Type));
            return Expression.NotEqual(member, constantExpression);

        }
        private static Expression BuildGreaterThanExpression(MemberExpression member, object value)
        {

            if (value == null)
            {
                return Expression.Constant(null);
            }

            if (value is Expression expressionValue)
            {
                return expressionValue;
            }

            if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableType = member.Type.GetGenericArguments()[0];
                var constantExpression = Expression.Constant(Convert.ChangeType(value, nullableType), member.Type);
                return Expression.GreaterThan(member, constantExpression);
            }
            else
            {
                var constantExpression = Expression.Constant(Convert.ChangeType(value, member.Type));
                return Expression.GreaterThan(member, constantExpression);
            }

        }
        private static Expression BuildGreaterThanOrEqualExpression(MemberExpression member, object value)
        {

            if (value == null)
            {
                return Expression.Constant(null);
            }

            if (value is Expression expressionValue)
            {
                return expressionValue;
            }

            if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableType = member.Type.GetGenericArguments()[0];
                var constantExpression = Expression.Constant(Convert.ChangeType(value, nullableType), member.Type);
                return Expression.GreaterThanOrEqual(member, constantExpression);
            }
            else
            {
                var constantExpression = Expression.Constant(Convert.ChangeType(value, member.Type));
                return Expression.GreaterThanOrEqual(member, constantExpression);
            }

        }
        private static Expression BuildLessThanExpression(MemberExpression member, object value)
        {

            if (value == null)
            {
                return Expression.Constant(null);
            }

            if (value is Expression expressionValue)
            {
                return expressionValue;
            }

            if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableType = member.Type.GetGenericArguments()[0];
                var constantExpression = Expression.Constant(Convert.ChangeType(value, nullableType), member.Type);
                return Expression.LessThan(member, constantExpression);
            }
            else
            {
                var constantExpression = Expression.Constant(Convert.ChangeType(value, member.Type));
                return Expression.LessThan(member, constantExpression);
            }

        }
        private static Expression BuildLessThanOrEqualExpression(MemberExpression member, object value)
        {

            if (value == null)
            {
                return Expression.Constant(null);
            }

            if (value is Expression expressionValue)
            {
                return expressionValue;
            }

            if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableType = member.Type.GetGenericArguments()[0];
                var constantExpression = Expression.Constant(Convert.ChangeType(value, nullableType), member.Type);
                return Expression.LessThanOrEqual(member, constantExpression);
            }
            else
            {
                var constantExpression = Expression.Constant(Convert.ChangeType(value, member.Type));
                return Expression.LessThanOrEqual(member, constantExpression);
            }

        }
        private static Expression BuildBetweenExpression(Expression member, Tuple<object, object> value)
        {
            return BuildBetweenExpression(member, value, false);
        }
        private static Expression BuildBetweenExpression(Expression member, Tuple<object, object> value, bool negate)
        {
            ConstantExpression constant1, constant2;

            Type typeOfFirstElement = value.Item1.GetType();
            var nullableType = member.Type.GetGenericArguments()[0];

            if (typeOfFirstElement == typeof(DateTime))
            {
                constant1 = Expression.Constant(value.Item1);
                constant2 = Expression.Constant(value.Item2);
            }
            else if (typeOfFirstElement == typeof(int) || typeOfFirstElement == typeof(double))
            {
                constant1 = Expression.Constant(Convert.ChangeType(value.Item1, nullableType), member.Type);
                constant2 = Expression.Constant(Convert.ChangeType(value.Item2, nullableType), member.Type);
            }
            else
            {
                throw new ArgumentException(ErrorMessage.UnsupportedTypeForBetweenOperator.Message);
            }


            var greaterThanOrEqual = Expression.GreaterThanOrEqual(member, constant1);
            var lessThanOrEqual = Expression.LessThanOrEqual(member, constant2);
            var andAlsoExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
            return negate ? Expression.Not(andAlsoExpression) : andAlsoExpression;



        }
        private static Expression BuildNotBetweenExpression(Expression member, Tuple<object, object> value)
        {
            return BuildBetweenExpression(member, value, true);
        }
        public static IQueryable<T> BuildFilters<T>(Dictionary<string, string> dicFilter, IQueryable<T> query)
        {
            var filters = ParseFilters(dicFilter);

            foreach (var filter in filters)
            {
                var propertyInfo = typeof(T).GetProperties().First(p => p.Name.Equals(filter.Key, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo != null)
                {
                    var filterExpression = BuildExpression<T>(filter.Key, filter.Value.Item1, filter.Value.Item2);
                    query = query.Where(filterExpression);
                }
            }

            return query;
        }

        public static (int PageNumber, int PageSize) BuildPaginationFilters(Dictionary<string, string> dicFilter)
        {
            int PageNumber = 0;
            int PageSize = 10;

            string fieldPageNumber = $"pagenumber";
            string fieldPageSize = $"pagesize";

            if (dicFilter.TryGetValue(fieldPageNumber, out string valuePageNumber) && int.TryParse(valuePageNumber, out int parsedPageNumber))
                PageNumber = parsedPageNumber;

            if (dicFilter.TryGetValue(fieldPageSize, out string valuePageSize) && int.TryParse(valuePageSize, out int parsedPageSize))
                PageSize = parsedPageSize;

            PageNumber = Math.Max(PageNumber, 1);

            return (PageNumber, PageSize);
        }

        public static async Task<ResultPaged<List<TDto>>> GetAllQueryBuilder<TModel, TDto>(
        IQueryable<TModel> query,
        Dictionary<string, string> filters,
        Expression<Func<TModel, TDto>> selector)
        where TModel : class
        where TDto : class
        {
            var typeQuery = query.ElementType;
            const string fieldDeletedAt = "DeletedAt";
            query = query.Where(x => EF.Property<DateTime?>(x, fieldDeletedAt) == null);
            query = BuildFilters(filters, query);

            var (PageNumber, PageSize) = BuildPaginationFilters(filters);

            var count = await query.CountAsync();

            PageNumber = Math.Max(PageNumber, 1);

            const string fieldId = "Id";
            var items = await query
                .OrderBy(x => EF.Property<int>(x, fieldId))
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(selector)
                .TagWithCallSite()
                .ToListAsync();

            return ResultService.OK(items, count, PageNumber, PageSize);
        }
    }
}
