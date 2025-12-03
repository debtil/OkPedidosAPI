using OkPedidos.Core.Result;
using OkPedidos.Models.DTOs.Request.User;
using OkPedidos.Models.DTOs.Response.User;

namespace OkPedidos.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<CreateUserResponse>> Create(CreateUserRequest request);
        Task<Result<CreateUserResponse>> Update(int id, CreateUserRequest request);
        Task<Result<CreateUserResponse>> Delete(int id);
        Task<Result<CreateUserResponse>> GetById(int id);
        Task<ResultPaged<List<CreateUserResponse>>> GetAll(Dictionary<string, string> filters);
    }
}
