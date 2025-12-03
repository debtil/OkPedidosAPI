using System.ComponentModel.DataAnnotations;
using static OkPedidosAPI.Helpers.Enums;

namespace OkPedidos.Models.DTOs.Request.User
{
    public class CreateUserRequest
    {
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user. Must be a valid email format and between 1 and 255 characters.
        /// </summary>
        [StringLength(255)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user. Must be at least 8 characters long.
        /// </summary>
        [MinLength(8)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the role of the user. This property is required and must be a valid enum value of type RoleUser.
        /// </summary>
        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }

        public int CompanyId { get; set; }
    }
}
