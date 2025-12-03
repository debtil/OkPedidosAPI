using OkPedidos.Core.Result;
using OkPedidos.Models.DTOs.Request.Company;
using OkPedidos.Models.DTOs.Response.Company;
using OkPedidos.Models.DTOs.Response.User;

namespace OkPedidos.Core.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Result<CreateCompanyResponse>> Create(CreateCompanyRequest request);
        Task<Result<CreateCompanyResponse>> Update(int id, CreateCompanyRequest request);
        Task<Result<CreateCompanyResponse>> Delete(int id);
        Task<Result<CreateCompanyResponse>> GetById(int id);
        Task<ResultPaged<List<CreateCompanyResponse>>> GetAll(Dictionary<string, string> filters);
        Task<Result<List<CreateUserResponse>>> GetCompanyEmployees(int companyId);
    }
}
