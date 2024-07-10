using Application.Common.Domain.Options;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MongoDB.Driver;

namespace Application.Common.Persistence;

public static class DependencyInjection
{
    public static void AddEfCore(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, EventPublisher>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
    }

    public static void AddMongoDbCache(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<MongoOptions>(configuration.GetSection("MongoSettings:Cache"));
        var connectionString = configuration["MongoSettings:Cache:ConnectionString"];
        services.AddSingleton<IMongoClient>((ctx) => new MongoClient(connectionString));
        services.AddScoped<IMongoDbCache, MongoDbCache>();
    }
}