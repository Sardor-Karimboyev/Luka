using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Luka;

public interface ILukaBuilder
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
    bool TryRegister(string name);
    void AddBuildAction(Action<IServiceProvider> execute);
    IServiceProvider Build();
}