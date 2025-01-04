using System.Reflection;

namespace Norimsoft.JsonRpc;

public class JsonRpcConfiguration
{
    private Assembly[] _methodsAssemblies = [];

    internal JsonRpcConfiguration()
    {
    }
    
    internal IEnumerable<Assembly> MethodsAssemblies => _methodsAssemblies;

    public JsonRpcConfiguration RegisterMethodsFrom(params Assembly[] assemblies)
    {
        _methodsAssemblies = assemblies;
        return this;
    }
}
