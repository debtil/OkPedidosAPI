using Microsoft.EntityFrameworkCore;
using OkPedidos.Core.Data;
using OkPedidos.Core.Result;
using OkPedidos.Core.Services.Base;
using OkPedidos.Core.Services.Interfaces;
using OkPedidos.Models.DTOs.Request.User;
using OkPedidos.Models.DTOs.Response.User;
using OkPedidos.Models.Models.Base;
using OkPedidos.Shared.Constants.Models;
using OkPedidosAPI;
using System.Net;

namespace OkPedidos.Core.Services.User
{
    public class UserService(OkPedidosDbContext _dbContext) : IUserService
    {
        public async Task<Result<CreateUserResponse>> Create(CreateUserRequest request, IdentityUserModel currrentUser)
        {
            try
            {
                var companyId = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == request.CompanyId && x.DeletedAt == null);
                if (companyId == null)
                    return ResultService.OK<CreateUserResponse>(HttpStatusCode.BadRequest, ErrorMessage.CompanyNotFound);

                UserModel user = await _dbContext.User.FirstOrDefaultAsync(x => x.Email == request.Email && x.Name == request.Name);
                
                if(user != null && user.DeletedAt == null)
                    return ResultService.OK<CreateUserResponse>(HttpStatusCode.BadRequest, ErrorMessage.RecordAlreadyExists);

                UserModel item = request;

                if (user?.DeletedAt != null)
                {
                    item.Id = user.Id;
                    item.Name = user.Name;
                    item.Email = user.Email;
                    item.Role = user.Role;
                    item.Password = user.Password;
                    item.UpdatedAt = DateTime.UtcNow;
                    item.UpdatedBy = currrentUser.UserId;
                    item.DeletedAt = null;
                    item.DeletedBy = null;

                    _dbContext.User.Update(item);
                }
                else
                {
                    item.CreatedAt = DateTime.Now;
                    item.CreatedBy = currrentUser.UserId;
                    await _dbContext.User.AddAsync(item);
                }

                await _dbContext.SaveChangesAsync();

                CreateUserResponse response = item;
                return ResultService.OK(HttpStatusCode.Created, response, InfoMessages.RecordCreatedSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateUserResponse>(ex);
            }
        }

        public async Task<Result<CreateUserResponse>> Update(int id, CreateUserRequest request, IdentityUserModel currrentUser)
        {
            try
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                    return ResultService.OK<CreateUserResponse>(HttpStatusCode.BadRequest, ErrorMessage.UserNotFound);

                var companyId = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == request.CompanyId && x.DeletedAt == null);
                if (companyId == null)
                    return ResultService.OK<CreateUserResponse>(HttpStatusCode.BadRequest, ErrorMessage.CompanyNotFound);

                user.Name = request.Name;
                user.Email = request.Email;
                user.Password = request.Password;
                user.Role = request.Role;
                user.CompanyId = request.CompanyId;

                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = currrentUser.UserId;

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

        public async Task<Result<CreateUserResponse>> Delete(int id, IdentityUserModel currrentUser)
        {
            try
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                    return ResultService.OK<CreateUserResponse>(HttpStatusCode.BadRequest, ErrorMessage.UserNotFound);

                user.DeletedAt = DateTime.Now;
                user.DeletedBy = currrentUser.UserId;
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
                    return ResultService.OK<CreateUserResponse>(HttpStatusCode.BadRequest, ErrorMessage.UserNotFound);

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
                        CreatedBy = x.CreatedBy,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy,
                        DeletedAt = x.DeletedAt,
                        DeletedBy = x.DeletedBy
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
