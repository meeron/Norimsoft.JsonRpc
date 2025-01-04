using System.Text.Json.Serialization;

namespace Norimsoft.JsonRpc;

public interface IJsonRpcResponse;

internal abstract class JsonRpcResponseBase : IJsonRpcResponse
{
    protected JsonRpcResponseBase(string id)
    {
        Id = id;
    }

    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; } = "2.0";

    public string Id { get; }
}

internal class JsonRpcResponseOk : JsonRpcResponseBase
{
    internal JsonRpcResponseOk(string id, object result)
        : base(id)
    {
        Result = result;
    }

    public object Result { get; }
}

internal class JsonRpcResponseError : JsonRpcResponseBase
{
    internal JsonRpcResponseError(string id, Error error)
        : base(id)
    {
        Error = error;
    }

    public Error Error { get; }
}

internal record Error(int Code, string Message, object? Data);
