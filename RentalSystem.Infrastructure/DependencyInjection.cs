using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentalSystem.Application.Common.Interfaces;
using RentalSystem.Infrastructure.Persistence;
using RentalSystem.Infrastructure.Security;

namespace RentalSystem.Infrastructure;

public static class DependencyInjection
{
     public static IServiceCollection AddInfrastructure(this IServiceCollection services,
     IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new  InvalidOperationException(
     "Connection string 'DefaultConnection' is not configured.");

     services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
     services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
     services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
     services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
     services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
     services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();

     return services;
  }
}