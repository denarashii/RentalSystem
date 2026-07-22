using RentalSystem.Application.Common.Interfaces;
using RentalSystem.Infrastructure;
using RentalSystem.Infrastructure.Persistence;
using RentalSystem.Infrastructure.Persistence.Seed;
using RentalSystem.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); 
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await DataSeeder.SeedInitialOwnerAsync(db, hasher, config, logger);
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
 
