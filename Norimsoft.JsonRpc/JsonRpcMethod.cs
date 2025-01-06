using System.Text.Json;
using Norimsoft.JsonRpc.Internal;

namespace Norimsoft.JsonRpc;

public abstract class JsonRpcMethodBase
{
    private JsonRpcRequest? _request;

    protected IJsonRpcResponse Ok(object result) =>
        new JsonRpcResponseOk(_request!.Id!.Value, result);
    
    protected IJsonRpcResponse Error(ushort code, string message, object? data = null) =>
        new JsonRpcResponseError(_request!.Id!.Value, new RpcError(code, message, data));
    
    internal IJsonRpcResponse Error(RpcError error) =>
        new JsonRpcResponseError(_request!.Id!.Value, error);

    internal virtual Task<IJsonRpcResponse> HandleInternal(JsonRpcRequest request, CancellationToken ct)
    {
        _request = request;
        return Task.FromResult(Ok(new {}));
    }
}

public abstract class JsonRpcMethod : JsonRpcMethodBase
{
    public abstract Task<IJsonRpcResponse> Handle(CancellationToken ct);

    internal override Task<IJsonRpcResponse> HandleInternal(JsonRpcRequest request, CancellationToken ct)
    {
        base.HandleInternal(request, ct);
        return Handle(ct);
    }
}

public abstract class JsonRpcMethod<TParam> : JsonRpcMethodBase
    where TParam : class
{
    public abstract Task<IJsonRpcResponse> Handle(TParam param, CancellationToken ct);

    internal override Task<IJsonRpcResponse> HandleInternal(JsonRpcRequest request, CancellationToken ct)
    {
        base.HandleInternal(request, ct);

        if (request.Params == null)
        {
            return Task.FromResult(Error(RpcError.InvalidRequest("Parameter is required")));
        }
        
        var param = request.Params!.Value.Deserialize<TParam>();
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
