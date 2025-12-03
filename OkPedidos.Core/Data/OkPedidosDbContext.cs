using Microsoft.EntityFrameworkCore;
using OkPedidos.Models.Models;
using OkPedidosAPI;

namespace OkPedidos.Core.Data
{
    public class OkPedidosDbContext : DbContext
    {
        public OkPedidosDbContext(DbContextOptions<OkPedidosDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<UserModel> User { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
    }
}
