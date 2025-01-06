using System.Text.Json;
using System.Text.Json.Serialization;

namespace Norimsoft.JsonRpc;

public interface IJsonRpcResponse;

internal abstract class JsonRpcResponseBase : IJsonRpcResponse
{
    protected JsonRpcResponseBase(JsonElement? id)
    {
        Id = id;
    }

    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; } = "2.0";

    public JsonElement? Id { get; }
}

internal class JsonRpcResponseOk : JsonRpcResponseBase
{
    internal JsonRpcResponseOk(JsonElement id, object result)
        : base(id)
    {
        Result = result;
    }

    public object Result { get; }
}

internal class JsonRpcResponseError : JsonRpcResponseBase
{
    internal JsonRpcResponseError(JsonElement id, RpcError rpcError)
        : base(id)
    {
        Error = rpcError;
    }
    
    internal JsonRpcResponseError(RpcError rpcError)
        : base(null)
    {
        Error = rpcError;
    }

    public RpcError Error { get; }
}

internal record RpcError(int Code, string Message, object? Data)
{
    internal static RpcError ParseError(object? data = null) => new(-32700, "Parse error", data);
    internal static RpcError InvalidRequest(object? data = null) => new(-32600, "Invalid Request", data);
    internal static RpcError ServerError(Exception? ex = null) =>
        new(-32000, "Server error", ex != null ? new { ex.Message, ex.StackTrace } : null);
    internal static RpcError MethodNotFound() => new(-32601, "Method not found", null);
}
