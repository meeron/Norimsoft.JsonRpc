using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Norimsoft.JsonRpc;

public static class ServiceCollectionExtensions
{
    public static void AddJsonRpc(this IServiceCollection services, Action<JsonRpcConfiguration> configure)
    {
        var config = new JsonRpcConfiguration();
        configure(config);

        var methodsTypes = config.MethodsAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericType: false })
            .Where(t => t.BaseType != null && t.BaseType.BaseType == typeof(JsonRpcMethodBase))
            .ToArray();

        foreach (var type in methodsTypes)
        {
            var nameAttribute = type.GetCustomAttribute<JsonRpcMethodNameAttribute>();
            services.AddKeyedScoped(typeof(JsonRpcMethodBase), nameAttribute?.Name ?? type.Name, type);
        }
    }
}
