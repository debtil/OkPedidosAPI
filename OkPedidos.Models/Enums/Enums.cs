namespace OkPedidosAPI.Helpers
{
    public class Enums
    {
        public enum UserRole
        {
            ADMIN, 
            MANAGER,
            CUSTOMER
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
