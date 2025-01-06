using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Norimsoft.JsonRpc.Internal;

namespace Norimsoft.JsonRpc;

public interface IJsonRpcResult : IResult;

internal abstract class JsonRpcResultBase : IJsonRpcResult
{
    protected JsonRpcResultBase(JsonElement? id)
    {
        Id = id;
    }

    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; } = "2.0";
    
    public JsonElement? Id { get; }
    
    protected abstract string GetJson();
    
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json";
        return httpContext.Response.WriteAsync(GetJson());
    }
}

internal class JsonRpcResultOk : JsonRpcResultBase
{
    internal JsonRpcResultOk(JsonElement id, object result)
        : base(id)
    {
        Result = result;
    }
    
    public object Result { get; }

    protected override string GetJson() => Json.Serialize(this);
}

internal class JsonRpcResultError : JsonRpcResultBase
{
    internal JsonRpcResultError(JsonElement id, RpcError rpcError)
        : base(id)
    {
        Error = rpcError;
    }
    
    internal JsonRpcResultError(RpcError rpcError)
        : base(null)
    {
        Error = rpcError;
    }
    
    public RpcError Error { get; }
    
    protected override string GetJson() => Json.Serialize(this);
}

internal record RpcError(int Code, string Message, object? Data)
{
    internal static RpcError ParseError(Exception ex) => new(-32700, "Parse error", GetDataFromException(ex));
    internal static RpcError InvalidRequest(object? data = null) => new(-32600, "Invalid Request", data);
    internal static RpcError ServerError(Exception? ex = null) =>
        new(-32000, "Server error", ex != null ? GetDataFromException(ex) : null);
    internal static RpcError MethodNotFound() => new(-32601, "Method not found", null);

    private static object GetDataFromException(Exception ex)
    {
        if (JsonRpcConfiguration.Environment?.IsProduction() ?? false)
        {
            return new { ex.Message };
        }
        
        return new { ex.Message, ex.StackTrace };
    }
}
