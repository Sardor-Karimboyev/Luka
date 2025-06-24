using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Luka.Persistence.Redis;

public static class Extensions
{
    private const string SectionName = "redis";
    public static ILukaBuilder AddRedis(this ILukaBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var options = builder.GetOptions<RedisOptions>(sectionName);
        return builder.AddRedis(options);
    }
    public static ILukaBuilder AddRedis(this ILukaBuilder builder, RedisOptions options)
    {
        builder.Services
            .AddSingleton(options)
            .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(options.ConnectionString))
            .AddTransient(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase(options.Database))
            .AddStackExchangeRedisCache(o =>
            {
                o.Configuration = options.ConnectionString;
                o.InstanceName = options.Instance;
            });

        return builder;
    }
}