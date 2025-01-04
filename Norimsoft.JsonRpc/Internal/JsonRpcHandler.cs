using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Norimsoft.JsonRpc.Internal;

internal static class JsonRpcHandler
{
    public static async Task<IResult> Handle(HttpContext ctx, CancellationToken ct)
    {
        await Task.CompletedTask;
        
        var (errorResponse, rpcRequest) = await ParseAndValidateRequest(ctx.Request.Body, ct);
        if (errorResponse != null)
        {
            return Results.Ok(errorResponse);
        }

        if (rpcRequest == null)
        {
            return Results.Ok(new JsonRpcResponseError(Error.ParseError("Not request found")));
        }

        if (rpcRequest.Id == null)
        {
            return Results.Ok(new JsonRpcResponseError(Error.ServerError("Notifications are not implemented.")));
        }
        
        var response = new JsonRpcResponseOk(rpcRequest.Id!.Value, "Hello World");
        
        return Results.Ok(response);
    }

    private static async Task<(IJsonRpcResponse?, JsonRpcRequest?)> ParseAndValidateRequest(Stream body, CancellationToken ct)
    {
        try
        {
            using var reader = new StreamReader(body);
            var bodyJson = await reader.ReadToEndAsync(ct);
            
            var req = JsonSerializer.Deserialize<JsonRpcRequest>(bodyJson)!;

            if (req.JsonRpc != "2.0")
            {
                return (new JsonRpcResponseError(Error.InvalidRequest("Invalid jsonrpc version")), null);    
            }

            if (string.IsNullOrWhiteSpace(req.Method))
            {
                return (new JsonRpcResponseError(Error.InvalidRequest("Missing method name")), null);
            }

            if (req.Method.StartsWith("rpc."))
            {
                return (new JsonRpcResponseError(Error.InvalidRequest("Cannot used internal rpc methods")), null);
            }

            if (req.Id != null && req.Id.Value.ValueKind != JsonValueKind.Number
                               && req.Id.Value.ValueKind != JsonValueKind.String)
            {
                return (new JsonRpcResponseError(Error.InvalidRequest("Invalid id. Must be either integer or string")), null);
            }

            if (req.Id is { ValueKind: JsonValueKind.Number } && !req.Id.Value.TryGetInt32(out _))
            {
                return (new JsonRpcResponseError(Error.InvalidRequest("Invalid id. Must either integer or string")), null);
            }
            
            return (null, req);
        }
        catch (Exception e)
        {
            // TODO: Configure exposing stack trace
            return (new JsonRpcResponseError(Error.ParseError(new { e.Message, e.StackTrace })), null);
        }
    }
}

internal class JsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = string.Empty;
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;
    [JsonPropertyName("id")]
    public JsonElement? Id { get; set; }
}
