using Microsoft.Extensions.DependencyInjection;
using OkPedidos.Core.Services.Company;
using OkPedidos.Core.Services.Interfaces;
using OkPedidos.Core.Services.User;

namespace OkPedidos.Core.DependencyInjection
{
    public static class CoreDependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();

            return services;
        }
    }
}
