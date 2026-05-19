using Backend.Data;
using Backend.Interfaces;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Repository;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Backend.Extensions;

// static allows us to use the methods inside this class without creating a new instance
public static class ApplicationServiceExtensions
{
  public static IServiceCollection AddApplicationServices(
    this IServiceCollection services,
    IConfiguration config
  )
  {
    services.AddControllers();

    // Database
    services.AddDbContext<DataContext>(opt =>
      {
        opt.UseNpgsql(
          config.GetConnectionString("DefaultConnection"),
          npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
        ).UseSnakeCaseNamingConvention();
      });

    // Cross-Origin Resource Sharing
    services.AddCors();

    // Dependency Injection Registration
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IGuitarRepository, GuitarRepository>();
    services.AddScoped<ILuthierRepository, LuthierRepository>();
    services.AddScoped<IBrandRepository, BrandRepository>();
    services.AddScoped<IRevisionMetadataRepository, RevisionMetadataRepository>();

    services.AddScoped<ITokenService, TokenService>(); // create once per http request
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IAccountService, AccountService>();
    services.AddScoped<IAdminService, AdminService>();
    services.AddScoped<IGuitarService, GuitarService>();
    services.AddScoped<ILuthierService, LuthierService>();
    services.AddScoped<IBrandService, BrandService>();
    services.AddScoped<IRevisionMetadataService, RevisionMetadataService>();

    // API Documentation
    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "GuitarDB API",
        Version = "v1"
      });
      c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
      {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer <token>' in the input below.\r\n\r\nExample: \"Bearer fulltokenhere\"",
      });
      c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    });

    return services;
  }
}
