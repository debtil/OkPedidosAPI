using OkPedidos.Models.DTOs.Request.User;
using OkPedidos.Models.Models;
using OkPedidos.Models.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OkPedidosAPI.Helpers.Enums;

namespace OkPedidosAPI
{
    public class UserModel : BaseModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]

        public CompanyModel Company { get; set; }

        public static implicit operator UserModel(CreateUserRequest value)
        {
            UserModel user = new()
            {
                Name = value.Name,
                Email = value.Email,
                Password = value.Password,
                Role = value.Role,
                CompanyId = value.CompanyId
            };

            return user;
        }
    }
}
