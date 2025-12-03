using Microsoft.EntityFrameworkCore;
using OkPedidos.Core.Data;
using OkPedidos.Core.DependencyInjection;
using OkPedidosAPI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<OkPedidosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("OkPedidos.Core")));

// Add services to the container.

builder.Services.AddCore();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

MigrationService.InitializeMigration(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
