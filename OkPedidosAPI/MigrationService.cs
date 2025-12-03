using Microsoft.EntityFrameworkCore;
using OkPedidos.Core.Data;

namespace OkPedidosAPI
{
    public class MigrationService
    {
        public static void InitializeMigration(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            serviceScope.ServiceProvider.GetService<OkPedidosDbContext>()!.Database.Migrate();
        }
    }
}
