using CleanArchitecture_2025.Domain.Users;
using CleanArchitecture_2025.Infrastructure.Context;
using GenericRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace CleanArchitecture_2025.Infrastructure;
public static class InfrastructureRegistrar
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            string connectionString = configuration.GetConnectionString("SqlServer")!;
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());

        services // Bu kod user manager kullanabilmek için yazılıyor. User Manager classının DI'sı.
            .AddIdentity<AppUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        // Bunun yerine scrutor kütüphanesini kullanabiliriz.

        services.Scan(options => options
        .FromAssemblies(typeof(InfrastructureRegistrar).Assembly)
        .AddClasses(publicOnly: false)
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        );
        // Bu kod servis ve repository için otomatik dependency injection yapar.



        return services;
    }
}
