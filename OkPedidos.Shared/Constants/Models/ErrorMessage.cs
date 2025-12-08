namespace OkPedidos.Shared.Constants.Models
{
    public static class ErrorMessage
    {
        public static readonly MessageDetail RecordNotFound = new("ER-001", "Record not found.");
        public static readonly MessageDetail UnsupportedTypeForBetweenOperator = new("ER-002", "Unsupported Type For Between Operator");
        public static readonly MessageDetail InvalidFormatForBetweenOperator = new("Er-003", "InvalidFormat For Between Operator");
        public static readonly MessageDetail CompanyNotFound = new("ER-004", "Company not found.");
        public static readonly MessageDetail RecordAlreadyExists = new("ER-005", "The record already exists.");
        public static readonly MessageDetail MissingEmailOrPassword = new("ER-006", "Email and Password are required for login.");
        public static readonly MessageDetail UserNotFound = new("ER-007", "User not found.");
        public static readonly MessageDetail InvalidTokenExpiration = new("ER-008", "Invalid token expiration in configuration.");
    }
}
