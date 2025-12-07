namespace OkPedidos.Models.DTOs.Response.User
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? AccessToken { get; set; }
        public int? Expiration { get; set; }
    }
}
