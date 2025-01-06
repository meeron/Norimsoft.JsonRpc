using System.Text.Json;

namespace Norimsoft.JsonRpc.Internal;

internal static class JsonRpcRequestParser
{
    internal static async Task<(IJsonRpcResult?, JsonRpcRequest?)> ParseAndValidate(
        Stream body,
        CancellationToken ct)
    {
        try
        {
            using var reader = new StreamReader(body);
            var bodyJson = await reader.ReadToEndAsync(ct);
            
            var req = Json.Deserialize<JsonRpcRequest>(bodyJson)!;

            if (req.JsonRpc != "2.0")
            {
                return (new JsonRpcResultError(RpcError.InvalidRequest("Invalid jsonrpc version")), null);    
            }

            if (string.IsNullOrWhiteSpace(req.Method))
            {
                return (new JsonRpcResultError(RpcError.InvalidRequest("Missing method name")), null);
            }

            if (req.Method.StartsWith("rpc."))
            {
                return (new JsonRpcResultError(RpcError.InvalidRequest("Cannot used internal rpc methods")), null);
            }

            if (req.Id != null && req.Id.Value.ValueKind != JsonValueKind.Number
                               && req.Id.Value.ValueKind != JsonValueKind.String)
            {
                return (new JsonRpcResultError(RpcError.InvalidRequest("Invalid id. Must be either integer or string")), null);
            }

            if (req.Id is { ValueKind: JsonValueKind.Number } && !req.Id.Value.TryGetInt32(out _))
            {
                return (new JsonRpcResultError(RpcError.InvalidRequest("Invalid id. Must either integer or string")), null);
            }
            
            if (req.Params != null && req.Params.Value.ValueKind != JsonValueKind.Object
                                   && req.Params.Value.ValueKind != JsonValueKind.Array)
            {
                return (new JsonRpcResultError(RpcError.InvalidRequest("Invalid params. Must be either array or object")), null);
            }
            
            return (null, req);
        }
        catch (Exception ex)
        {
            return (new JsonRpcResultError(RpcError.ParseError(ex)), null);
        }
    }
}
