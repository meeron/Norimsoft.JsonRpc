using System.Text.Json;
using Norimsoft.JsonRpc.Internal;

namespace Norimsoft.JsonRpc;

public abstract class JsonRpcMethodBase
{
    private JsonRpcRequest? _request;

    protected IJsonRpcResult Ok(object result) =>
        new JsonRpcResultOk(_request!.Id!.Value, result);
    
    protected IJsonRpcResult Error(ushort code, string message, object? data = null) =>
        new JsonRpcResultError(_request!.Id!.Value, new RpcError(code, message, data));
    
    internal IJsonRpcResult Error(RpcError error) =>
        new JsonRpcResultError(_request!.Id!.Value, error);

    internal virtual Task<IJsonRpcResult> HandleInternal(JsonRpcRequest request, CancellationToken ct)
    {
        _request = request;
        return Task.FromResult(Ok(new {}));
    }
}

public abstract class JsonRpcMethod : JsonRpcMethodBase
{
    public abstract Task<IJsonRpcResult> Handle(CancellationToken ct);

    internal override Task<IJsonRpcResult> HandleInternal(JsonRpcRequest request, CancellationToken ct)
    {
        base.HandleInternal(request, ct);
        return Handle(ct);
    }
}

public abstract class JsonRpcMethod<TParam> : JsonRpcMethodBase
    where TParam : class
{
    public abstract Task<IJsonRpcResult> Handle(TParam param, CancellationToken ct);

    internal override Task<IJsonRpcResult> HandleInternal(JsonRpcRequest request, CancellationToken ct)
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
