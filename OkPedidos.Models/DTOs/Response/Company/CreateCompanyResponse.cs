using OkPedidos.Models.Models;

namespace OkPedidos.Models.DTOs.Response.Company
{
    public class CreateCompanyResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }

        public static implicit operator CreateCompanyResponse(CompanyModel value)
        {
            CreateCompanyResponse item = new()
            {
                Id = value.Id,
                Name = value.Name,
                Phone = value.Phone,
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
