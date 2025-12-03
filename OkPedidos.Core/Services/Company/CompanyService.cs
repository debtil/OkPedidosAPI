using Microsoft.EntityFrameworkCore;
using OkPedidos.Core.Data;
using OkPedidos.Core.Result;
using OkPedidos.Core.Services.Base;
using OkPedidos.Core.Services.Interfaces;
using OkPedidos.Models.DTOs.Request.Company;
using OkPedidos.Models.DTOs.Response.Company;
using OkPedidos.Models.DTOs.Response.User;
using OkPedidos.Models.Models;
using OkPedidos.Shared.Constants.Models;
using OkPedidosAPI;
using System.Net;

namespace OkPedidos.Core.Services.Company
{
    public class CompanyService(OkPedidosDbContext _dbContext) : ICompanyService
    {
        public async Task<Result<CreateCompanyResponse>> Create(CreateCompanyRequest request)
        {
            try
            {
                CompanyModel company = request;
                company.CreatedAt = DateTime.Now;
                await _dbContext.Companies.AddAsync(company);
                await _dbContext.SaveChangesAsync();

                CreateCompanyResponse response = company;
                return ResultService.OK(HttpStatusCode.Created, response, InfoMessages.RecordCreatedSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateCompanyResponse>(ex);
            }
        }

        public async Task<Result<CreateCompanyResponse>> Update(int id, CreateCompanyRequest request)
        {
            try
            {
                var company = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == id);
                if (company == null)
                    return ResultService.NotFound<CreateCompanyResponse>();

                company.Name = request.Name;
                company.Phone = request.Phone;

                company.UpdatedAt = DateTime.Now;

                _dbContext.Companies.Update(company);
                await _dbContext.SaveChangesAsync();

                CreateCompanyResponse response = company;
                return ResultService.OK(HttpStatusCode.OK, response, InfoMessages.RecordUpdatedSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateCompanyResponse>(ex);
            }
        }

        public async Task<Result<CreateCompanyResponse>> Delete(int id)
        {
            try
            {
                var company = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == id);
                if (company == null)
                    return ResultService.NotFound<CreateCompanyResponse>();

                company.DeletedAt = DateTime.Now;
                _dbContext.Companies.Update(company);
                await _dbContext.SaveChangesAsync();

                CreateCompanyResponse response = company;
                return ResultService.OK(HttpStatusCode.OK, response, InfoMessages.RecordDeletedSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateCompanyResponse>(ex);
            }
        }

        public async Task<Result<CreateCompanyResponse>> GetById(int id)
        {
            try
            {
                var query = _dbContext.Set<CompanyModel>()
                     .Where(x => x.Id == id)
                     .Where(x => x.DeletedAt == null);

                var item = await query
                    .TagWithCallSite()
                    .FirstOrDefaultAsync();

                if (item is null)
                    return ResultService.NotFound<CreateCompanyResponse>();

                CreateCompanyResponse response = item;

                return ResultService.OK(HttpStatusCode.OK, response, InfoMessages.OperationSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<CreateCompanyResponse>(ex);
            }
        }

        public async Task<ResultPaged<List<CreateCompanyResponse>>> GetAll(Dictionary<string, string> filters)
        {
            try
            {
                var query = _dbContext.Set<CompanyModel>().AsQueryable();
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
                     .Select(x => new CreateCompanyResponse
                     {
                         Id = x.Id,
                         Name = x.Name,
                         Phone = x.Phone,
                         CreatedAt = x.CreatedAt,
                         UpdatedAt = x.UpdatedAt,
                         DeletedAt = x.DeletedAt,
                     })
                     .ToListAsync();

                return ResultService.OK(items, count, PageNumber, PageSize);
            }
            catch (Exception ex)
            {
                return ResultService.ErrorPaged<CreateCompanyResponse>(ex);
            }
        }

        public async Task<Result<List<CreateUserResponse>>> GetCompanyEmployees(int companyId)
        {
            try
            {
                var companyExists = await _dbContext.Companies
                    .AnyAsync(c => c.Id == companyId && c.DeletedAt == null);

                if (!companyExists)
                    return ResultService.NotFound<List<CreateUserResponse>>();

                var employees = await _dbContext.Set<UserModel>()
                    .Where(u => u.CompanyId == companyId)
                    .Where(u => u.DeletedAt == null)
                    .OrderBy(u => u.Name)
                    .Select(u => new CreateUserResponse
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Role = u.Role,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        DeletedAt = u.DeletedAt
                    })
                    .ToListAsync();

                return ResultService.OK(HttpStatusCode.OK, employees, InfoMessages.OperationSuccessful);
            }
            catch (Exception ex)
            {
                return ResultService.Error<List<CreateUserResponse>>(ex);
            }
        }
    }
}
