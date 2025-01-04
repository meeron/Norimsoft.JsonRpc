using Norimsoft.JsonRpc.Internal;

namespace Norimsoft.JsonRpc;

public abstract class JsonRpcMethodBase
{
    private JsonRpcRequest? _request;
    
    protected virtual string Name => GetType().Name;

    protected IJsonRpcResponse Ok(object result) =>
        new JsonRpcResponseOk(_request!.Id!.Value, result);
    
    protected IJsonRpcResponse Error(ushort code, string message, object? data = null) =>
        new JsonRpcResponseError(_request!.Id!.Value, new Error(code, message, data));

    internal void SetRequest(JsonRpcRequest request)
    {
        _request = request;
    }
}

public abstract class JsonRpcMethod : JsonRpcMethodBase
{
    public abstract Task<IJsonRpcResponse> Handle(CancellationToken ct);
}

public abstract class JsonRpcMethod<TParam> : JsonRpcMethodBase
    where TParam : class
{
    public abstract Task<IJsonRpcResponse> Handle(TParam param, CancellationToken ct);
    
}
