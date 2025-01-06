using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Norimsoft.JsonRpc;

public class JsonRpcConfiguration
{
    private Assembly[] _methodsAssemblies = [];
    private static IHostEnvironment? _environment;

    internal JsonRpcConfiguration()
    {
    }
    
    internal IEnumerable<Assembly> MethodsAssemblies => _methodsAssemblies;
    internal static IHostEnvironment? Environment => _environment;

    public JsonRpcConfiguration RegisterMethodsFrom(params Assembly[] assemblies)
    {
        _methodsAssemblies = assemblies;
        return this;
    }

    public JsonRpcConfiguration UseEnvironment(IHostEnvironment environment)
    {
        _environment = environment;
        return this;
    }
}
