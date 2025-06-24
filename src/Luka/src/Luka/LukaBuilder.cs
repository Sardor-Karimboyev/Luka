using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luka;

public class LukaBuilder : ILukaBuilder
{
    private readonly ConcurrentDictionary<string, bool> _registry = new();
    private readonly List<Action<IServiceProvider>> _buildActions;
    private readonly IServiceCollection _services;
    
    IServiceCollection ILukaBuilder.Services => _services;
        
    public IConfiguration Configuration { get; }
    
    private LukaBuilder(IServiceCollection services, IConfiguration configuration)
    {
        _buildActions = new List<Action<IServiceProvider>>();
        _services = services;
        Configuration = configuration;
    }

    public static ILukaBuilder Create(IServiceCollection services, IConfiguration configuration = null)
        => new LukaBuilder(services, configuration);

    public bool TryRegister(string name) => _registry.TryAdd(name, true);

    public void AddBuildAction(Action<IServiceProvider> execute)
        => _buildActions.Add(execute);

    public IServiceProvider Build()
    {
        var serviceProvider = _services.BuildServiceProvider();
        _buildActions.ForEach(a => a(serviceProvider));
        return serviceProvider;
    }
}