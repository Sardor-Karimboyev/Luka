using Luka.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luka;

public static class Extensions
{
    private const string SectionName = "app";

    public static ILukaBuilder AddLuka(this IServiceCollection services, string sectionName = SectionName,
        IConfiguration configuration = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var builder = LukaBuilder.Create(services, configuration);
        var options = builder.GetOptions<AppOptions>(sectionName);
        builder.Services.AddMemoryCache();
        services.AddSingleton(options);
        if (!options.DisplayBanner || string.IsNullOrWhiteSpace(options.Name))
        {
            return builder;
        }

        var version = options.DisplayVersion ? $" {options.Version}" : string.Empty;
        Console.WriteLine(Figgle.FiggleFonts.Doom.Render($"{options.Name}{version}"));

        return builder;
    }

    public static IApplicationBuilder UseLuka(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        return app;
    }

    public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
        where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(sectionName).Bind(model);
        return model;
    }

    public static TModel GetOptions<TModel>(this ILukaBuilder builder, string settingsSectionName)
        where TModel : new()
    {
        if (builder.Configuration is not null)
        {
            return builder.Configuration.GetOptions<TModel>(settingsSectionName);
        }

        using var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<TModel>(settingsSectionName);
    }

    public static string Underscore(this string value)
        => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
            .ToLowerInvariant();
}