namespace OkPedidos.Shared.Constants.Models
{
    public class MessageDetail(string code, string message)
    {
        public string Code { get; set; } = code;
        public string Message { get; set; } = message;
    }
}
