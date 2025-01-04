using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Norimsoft.JsonRpc;

public static class ServiceCollectionExtensions
{
    public static void AddJsonRpc(this IServiceCollection services)
    {
        var asm = Assembly.GetCallingAssembly();
        services.AddJsonRpc(c => c.RegisterMethodsFrom(asm));
    }

    public static void AddJsonRpc(this IServiceCollection services, Action<JsonRpcConfiguration> configure)
    {
        var config = new JsonRpcConfiguration();
        configure(config);

        var parameterlessMethodsTypes = config.MethodsAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericType: false })
            .Where(t => t.BaseType == typeof(JsonRpcMethod))
            .ToArray();

        foreach (var type in parameterlessMethodsTypes)
        {
            services.AddKeyedScoped(typeof(JsonRpcMethod), type.Name, type);
        }
    }
}
