namespace OkPedidos.Shared.Constants.Models
{
    public static class ErrorMessage
    {
        public static readonly MessageDetail RecordNotFound = new("ER-001", "Record not found.");
        public static readonly MessageDetail UnsupportedTypeForBetweenOperator = new("ER-002", "Unsupported Type For Between Operator");
        public static readonly MessageDetail InvalidFormatForBetweenOperator = new("Er-003", "InvalidFormat For Between Operator");
    }
}
