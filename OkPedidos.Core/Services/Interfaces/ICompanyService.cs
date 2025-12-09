using OkPedidos.Core.Result;
using OkPedidos.Models.DTOs.Request.Company;
using OkPedidos.Models.DTOs.Response.Company;
using OkPedidos.Models.DTOs.Response.User;
using OkPedidos.Models.Models.Base;

namespace OkPedidos.Core.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Result<CreateCompanyResponse>> Create(CreateCompanyRequest request, IdentityUserModel currentUser);
        Task<Result<CreateCompanyResponse>> Update(int id, CreateCompanyRequest request, IdentityUserModel currentUser);
        Task<Result<CreateCompanyResponse>> Delete(int id, IdentityUserModel currentUser);
        Task<Result<CreateCompanyResponse>> GetById(int id);
        Task<ResultPaged<List<CreateCompanyResponse>>> GetAll(Dictionary<string, string> filters);
        Task<Result<List<CreateUserResponse>>> GetCompanyEmployees(IdentityUserModel currentUser);
    }
}
