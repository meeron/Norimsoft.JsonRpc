using System.Text.Json;
using Norimsoft.JsonRpc.Internal;

namespace Norimsoft.JsonRpc;

public abstract class JsonRpcMethodBase
{
    private JsonRpcRequest? _request;

    internal JsonRpcRequest Request => _request!;

    protected IJsonRpcResponse Ok(object result) =>
        new JsonRpcResponseOk(_request!.Id!.Value, result);
    
    protected IJsonRpcResponse Error(ushort code, string message, object? data = null) =>
        new JsonRpcResponseError(_request!.Id!.Value, new Error(code, message, data));

    internal void SetRequest(JsonRpcRequest request)
    {
        _request = request;
    }
}

public abstract class JsonRpcMethodBaseParam : JsonRpcMethodBase
{
    internal abstract Task<IJsonRpcResponse> HandleInternal(CancellationToken ct);
}

public abstract class JsonRpcMethod : JsonRpcMethodBase
{
    public abstract Task<IJsonRpcResponse> Handle(CancellationToken ct);
}

public abstract class JsonRpcMethod<TParam> : JsonRpcMethodBaseParam
    where TParam : class
{
    public abstract Task<IJsonRpcResponse> Handle(TParam param, CancellationToken ct);

    internal override Task<IJsonRpcResponse> HandleInternal(CancellationToken ct)
    {
        var param = Request.Params!.Value.Deserialize<TParam>();
        return Handle(param!, ct);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class JsonRpcMethodNameAttribute : Attribute
{
    public JsonRpcMethodNameAttribute(string name)
    {
        Name = name;
    }

    internal string Name { get; }
}
