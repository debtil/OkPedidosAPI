using OkPedidos.Models.Models;

namespace OkPedidos.Models.DTOs.Response.Company
{
    public class CreateCompanyResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public static implicit operator CreateCompanyResponse(CompanyModel value)
        {
            CreateCompanyResponse item = new()
            {
                Id = value.Id,
                Name = value.Name,
                Phone = value.Phone,
                CreatedAt = value.CreatedAt,
                UpdatedAt = value.UpdatedAt,
                DeletedAt = value.DeletedAt,
            };

            return item;
        }
    }
}
