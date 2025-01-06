using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Norimsoft.JsonRpc.Internal;

internal static class JsonRpcHandler
{
    public static async Task<IResult> Handle(HttpContext ctx, CancellationToken ct)
    {
        try
        {
            var (errorResponse, rpcRequest) = await ParseAndValidateRequest(ctx.Request.Body, ct);
            if (errorResponse != null)
            {
                return Results.Ok(errorResponse);
            }

            if (rpcRequest == null)
            {
                return Results.Ok(new JsonRpcResponseError(RpcError.ParseError("Not request found")));
            }

            if (rpcRequest.Id == null)
            {
                return Results.Ok(new JsonRpcResponseError(
                    RpcError.ServerError(new NotImplementedException("Notifications are not implemented."))));
            }
        
            var rpcMethod = ctx.RequestServices.GetKeyedService<JsonRpcMethodBase>(rpcRequest.Method);
            if (rpcMethod == null)
            {
                return Results.Ok(new JsonRpcResponseError(RpcError.MethodNotFound()));
            }
            
            var response = await rpcMethod.HandleInternal(rpcRequest, ct);
        
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Ok(new JsonRpcResponseError(RpcError.ServerError(ex)));
        }
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
                return (new JsonRpcResponseError(RpcError.InvalidRequest("Invalid jsonrpc version")), null);    
            }

            if (string.IsNullOrWhiteSpace(req.Method))
            {
                return (new JsonRpcResponseError(RpcError.InvalidRequest("Missing method name")), null);
            }

            if (req.Method.StartsWith("rpc."))
            {
                return (new JsonRpcResponseError(RpcError.InvalidRequest("Cannot used internal rpc methods")), null);
            }

            if (req.Id != null && req.Id.Value.ValueKind != JsonValueKind.Number
                               && req.Id.Value.ValueKind != JsonValueKind.String)
            {
                return (new JsonRpcResponseError(RpcError.InvalidRequest("Invalid id. Must be either integer or string")), null);
            }

            if (req.Id is { ValueKind: JsonValueKind.Number } && !req.Id.Value.TryGetInt32(out _))
            {
                return (new JsonRpcResponseError(RpcError.InvalidRequest("Invalid id. Must either integer or string")), null);
            }
            
            if (req.Params != null && req.Params.Value.ValueKind != JsonValueKind.Object
                               && req.Params.Value.ValueKind != JsonValueKind.Array)
            {
                return (new JsonRpcResponseError(RpcError.InvalidRequest("Invalid params. Must be either array or object")), null);
            }
            
            return (null, req);
        }
        catch (Exception e)
        {
            // TODO: Configure exposing stack trace
            return (new JsonRpcResponseError(RpcError.ParseError(new { e.Message, e.StackTrace })), null);
        }
    }
}
