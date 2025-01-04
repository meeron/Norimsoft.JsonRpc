using Norimsoft.JsonRpc.Internal;

namespace Norimsoft.JsonRpc;

public abstract class JsonRpcMethodBase
{
    protected virtual string Name => GetType().Name;

    protected IJsonRpcResponse Ok(object result) =>
        new JsonRpcResponseOk("", result);
    
    protected IJsonRpcResponse Error(ushort code, string message, object? data = null) =>
        new JsonRpcResponseError("", new Error(code, message, data));
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
