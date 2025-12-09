using OkPedidosAPI;
using static OkPedidosAPI.Helpers.Enums;

namespace OkPedidos.Models.DTOs.Response.User
{
    public class CreateUserResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public UserRole Role { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        public static implicit operator CreateUserResponse(UserModel value)
        {
            CreateUserResponse item = new()
            {
                Id = value.Id,
                Name = value.Name,
                Email = value.Email,
                Role = value.Role,
                CompanyId = value.CompanyId,
                CreatedAt = value.CreatedAt,
                CreatedBy = value.CreatedBy,
                UpdatedAt = value.UpdatedAt,
                UpdatedBy = value.UpdatedBy,
                DeletedAt = value.DeletedAt,
                DeletedBy = value.DeletedBy
            };

            return item;
        }
    }
}
