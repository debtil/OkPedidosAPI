using OkPedidos.Models.DTOs.Request.Company;
using OkPedidos.Models.Models.Base;

namespace OkPedidos.Models.Models
{
    public class CompanyModel : BaseModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        public static implicit operator CompanyModel(CreateCompanyRequest request)
        {
            CompanyModel item = new()
            {
                Name = request.Name,
                Phone = request.Phone,
            };

            return item;
        }
    }
}
