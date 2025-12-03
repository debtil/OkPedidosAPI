using OkPedidos.Core.Data;
using OkPedidos.Core.Result;
using OkPedidos.Core.Services.Base;
using OkPedidos.Core.Services.Interfaces;
using OkPedidos.Models.DTOs.Request.User;
using OkPedidos.Models.DTOs.Response.User;
using OkPedidos.Shared.Constants.Models;
using OkPedidosAPI;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace OkPedidos.Core.Services.User
{
    public class UserService(OkPedidosDbContext _dbContext) : IUserService
    {
        public async Task<Result<CreateUserResponse>> Create(CreateUserRequest request)
        {
            try
            {
                UserModel user = request;
                user.CreatedAt = DateTime.Now;
                await _dbContext.User.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                CreateUserResponse response = user;
                return ResultService.OK(HttpStatusCode.Created, response, InfoMessages.RecordCreatedSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateUserResponse>(ex);
            }
        }

        public async Task<Result<CreateUserResponse>> Update(int id, CreateUserRequest request)
        {
            try
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                    return ResultService.NotFound<CreateUserResponse>();

                user.Name = request.Name;
                user.Email = request.Email;
                user.Password = request.Password;
                user.Role = request.Role;
                user.CompanyId = request.CompanyId;

                user.UpdatedAt = DateTime.Now;

                _dbContext.User.Update(user);
                await _dbContext.SaveChangesAsync();

                CreateUserResponse response = user;
                return ResultService.OK(HttpStatusCode.OK, response, InfoMessages.RecordUpdatedSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateUserResponse>(ex);
            }
        }

        public async Task<Result<CreateUserResponse>> Delete(int id)
        {
            try
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                    return ResultService.NotFound<CreateUserResponse>();

                user.DeletedAt = DateTime.Now;
                _dbContext.User.Update(user);
                await _dbContext.SaveChangesAsync();

                CreateUserResponse response = user;
                return ResultService.OK(HttpStatusCode.OK, response, InfoMessages.RecordDeletedSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateUserResponse>(ex);
            }
        }

        public async Task<Result<CreateUserResponse>> GetById(int id)
        {
            try
            {
                var query = _dbContext.Set<UserModel>()
                     .Where(x => x.Id == id)
                     .Where(x => x.DeletedAt == null);

                 var item = await query
                     .TagWithCallSite()
                     .FirstOrDefaultAsync();

                 if (item is null)
                     return ResultService.NotFound<CreateUserResponse>();

                 CreateUserResponse response = item;

                 return ResultService.OK(HttpStatusCode.OK, response, InfoMessages.OperationSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateUserResponse>(ex);
            }
        }

        public async Task<ResultPaged<List<CreateUserResponse>>> GetAll(Dictionary<string, string> filters)
        {
            try
            {
                var query = _dbContext.Set<UserModel>().AsQueryable();
                Type typeQuery = query.ElementType;

                query = query.Where(x => x.DeletedAt == null);
                query = FilterService.BuildFilters(filters, query);

                var (PageNumber, PageSize) = FilterService.BuildPaginationFilters(filters);

                var count = await query.CountAsync();

                PageNumber = Math.Max(PageNumber, 1);
                var items = await query
                     .OrderBy(x => x.Id)
                     .Skip((PageNumber - 1) * PageSize)
                     .Take(PageSize)
                     .Select(x => new CreateUserResponse
                     {
                        Id = x.Id,
                        Name = x.Name,
                        Email = x.Email,
                        Role = x.Role,
                        CompanyId = x.CompanyId,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        DeletedAt = x.DeletedAt,
                     })
                     .ToListAsync();

                return ResultService.OK(items, count, PageNumber, PageSize);
            }
            catch (Exception ex)
            {
                return ResultService.ErrorPaged<CreateUserResponse>(ex);
            }
        }
    }
}
