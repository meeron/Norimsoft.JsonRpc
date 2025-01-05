﻿using System.Reflection;
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
        
        var parameterMethodsTypes = config.MethodsAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericType: false })
            .Where(t => t.BaseType != null && t.BaseType.BaseType == typeof(JsonRpcMethodBaseParam))
            .ToArray();

        foreach (var type in parameterlessMethodsTypes)
        {
            var nameAttribute = type.GetCustomAttribute<JsonRpcMethodNameAttribute>();
            services.AddKeyedScoped(typeof(JsonRpcMethod), nameAttribute?.Name ?? type.Name, type);
        }
        
        foreach (var type in parameterMethodsTypes)
        {
            var nameAttribute = type.GetCustomAttribute<JsonRpcMethodNameAttribute>();
            services.AddKeyedScoped(typeof(JsonRpcMethodBaseParam), nameAttribute?.Name ?? type.Name, type);
        }
    }
}
