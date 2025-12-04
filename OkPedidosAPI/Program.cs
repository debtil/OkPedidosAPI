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
app.MapOpenApi(); // Enable OpenAPI endpoints
app.UseSwagger();  // Enable Swagger UI
app.UseSwaggerUI(c =>
{
    // Define Swagger UI options and the endpoint for Swagger JSON
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OkPedidos API v1");
});

// Redirect root to Swagger UI if requested from `/`
app.MapGet("/", () => Results.Redirect("/swagger"));

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
