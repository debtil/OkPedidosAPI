using static OkPedidosAPI.Helpers.Enums;

namespace OkPedidos.Models.Models.Base
{
    public class IdentityUserModel
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int? Company { get; set; }
        public UserRole Role { get; set; }
    }
}
