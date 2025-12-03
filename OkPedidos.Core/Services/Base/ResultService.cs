using Microsoft.AspNetCore.Mvc.ModelBinding;
using Npgsql;
using OkPedidos.Core.Result;
using OkPedidos.Models.Models.Base;
using OkPedidos.Shared.Constants.Models;
using OkPedidosAPI.Helpers;
using System.Net;

namespace OkPedidos.Core.Services.Base
{
    public static class ResultService
    {
        #region SUCESSS

        private static bool CheckOrigemSourceData<T>(object items)
        {
            if (items is List<T> itemList)
            {
                if (itemList.Count == 0)
                    return false;

                return GetIsFromCacheValue(itemList[0]);
            }
            else if (items is T singleItem)
            {
                return GetIsFromCacheValue(singleItem);
            }
            else
            {
                return false;
            }
        }

        private static bool GetIsFromCacheValue<T>(T item)
        {
            const string propertyName = "IsFromCache";
            var propertyInfo = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase), null);

            if (propertyInfo != null)
                return (bool)propertyInfo.GetValue(item);

            return false;
        }

        public static ResultPaged<List<T>> OK<T>(List<T> items, int count, int PageNumber, int PageSize)
        {
            bool useCache = CheckOrigemSourceData<T>(items);
            return new ResultPaged<List<T>>(items, count, PageNumber, PageSize, useCache);
        }

        public static Result<TData> OK<TData>(HttpStatusCode status, MessageDetail detail)
        {
            var ret = new Result<TData>(status, detail);
            return ret;
        }
        public static Result<TData> OK<TData>(HttpStatusCode status, TData item, MessageDetail detail)
        {
            bool useCache = CheckOrigemSourceData<TData>(item);
            var ret = new Result<TData>(item, status, detail)
            {
                SourceData = useCache ? $"cache" : $"database"
            };
            return ret;
        }
        public static Result<TData> OK<TData>(HttpStatusCode status, TData item)
        {
            bool useCache = CheckOrigemSourceData<TData>(item);
            var ret = new Result<TData>(item, status)
            {
                SourceData = useCache ? $"cache" : $"database"
            };
            return ret;
        }
        public static Result<TData> OK<TData>(Result<TData> response)
        {
            return response;

        }

        public static Result<TData> OK<TData>(HttpStatusCode status, MessageDetail detail, params string[] nameFields)
        {
            var ret = new Result<TData>(status, detail);
            for (int i = 0; i < nameFields.Length; i++)
            {
                var placeholder = $"{{{i}}}";
                ret.Message = detail.Message.Replace(placeholder, nameFields[i]);
            }
            return ret;
        }

        #endregion

        public static Result<TData> NotFound<TData>()
        {
            var ret = new Result<TData>(HttpStatusCode.NotFound, ErrorMessage.RecordNotFound);
            return ret;
        }

        public static ResultPaged<List<TData>> ErrorPaged<TData>(Exception ex)
        {
            LogService.Error(ex);
            const string title = "error";
            List<ErrorModel> errors = [new()
            {
                Title = title,
                Code = ex.HResult.ToString(),
                Source = ex.Source,
                Description = ex.Message,
                Type = Enums.ErrorType.BUSINESS,
            }];

            return new ResultPaged<List<TData>>(errors);
        }

        public static ResultPaged<List<TData>> ErrorPaged<TData>(Exception ex, string field)
        {
            LogService.Error(ex);
            const string title = "error";
            List<ErrorModel> errors = [new()
            {
                Title = title,
                Code = ex.HResult.ToString(),
                Field = field,
                Source = ex.Source,
                Description = ex.Message,
                Type = Enums.ErrorType.BUSINESS,
            }];

            return new ResultPaged<List<TData>>(errors);
        }
        public static Result<TData> Error<TData>(Exception ex)
        {
            string msg = string.Empty;
            List<ErrorModel> errors;

            LogService.Error(ex);

            if (ex.InnerException is PostgresException exPsql)
            {
                msg = exPsql.MessageText.ToString();
                errors = [new()
                {
                    Title = SettingsDomain.Application.TitleError,
                    Code = exPsql.SqlState,
                    Source = exPsql.Source,
                    Description = msg,
                    Type = Enums.ErrorType.DATABASE

                }];
            }
            else
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = ex.Message ?? ex.InnerException.Message ?? string.Empty;
                }
                else
                {
                    msg += ex.Message ?? ex.InnerException.Message ?? string.Empty;
                    msg = Environment.NewLine + msg;
                }

                errors = [new()
                {
                    Title =  SettingsDomain.Application.TitleError ?? string.Empty,
                    Code = ex.HResult.ToString(),
                    Source = ex.Source,
                    Description = msg,
                    Type= Enums.ErrorType.BUSINESS
                }];
            }

            var retError = new Result<TData>(HttpStatusCode.InternalServerError, errors);
            return retError;
        }

        public static Result<TData> Error<TData>(HttpStatusCode status)
        {
            List<ErrorModel> errors = [];
            var retError = new Result<TData>(status, errors)
            {
                StatusCode = status
            };

            return retError;
        }
        public static Result<TData> Error<TData>(ModelStateDictionary modelState, string sourceError, string numberError)
        {
            var errors = GetError(modelState, sourceError, numberError);
            var retError = new Result<TData>(HttpStatusCode.BadRequest, errors);
            return retError;
        }

        private static List<ErrorModel> GetError(ModelStateDictionary errors, string sourceError, string numberError)
        {
            List<ErrorModel> list = [];

            var lstErrors = errors.Where(ms => ms.Value.Errors.Any())
                                            .Select(x => new { x.Key, x.Value.Errors });

            foreach (var error in lstErrors)
            {
                var fieldKey = error.Key;

                foreach (var item in error.Errors)
                {
                    list.Add(new ErrorModel()
                    {
                        Title = $"Validation: [{fieldKey}]",
                        Code = numberError,
                        Source = sourceError,
                        Description = item.ErrorMessage,
                        Field = fieldKey,
                        Type = Enums.ErrorType.VALIDATION,
                    });
                }
            }

            return list;
        }
    }
}
