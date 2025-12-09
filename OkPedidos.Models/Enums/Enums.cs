using System.ComponentModel;

namespace OkPedidosAPI.Helpers
{
    public class Enums
    {
        public enum UserRole
        {
            [Description("Admin")]
            ADMIN = 1,

            [Description("Manager")]
            MANAGER = 2,

            [Description("Employee")]
            EMPLOYEE = 3
        }

        public enum ErrorType
        {
            VALIDATION = 1,
            BUSINESS = 2,
            DATABASE = 3
        }

        public enum ComparisonOperator
        {
            Equal,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
            Contains,
            NotContains,
            Between,
            NotBetween
        }
    }
}
