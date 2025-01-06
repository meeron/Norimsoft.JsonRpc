using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Norimsoft.JsonRpc.Internal;

internal static class JsonRpcHandler
{
    public static async Task<IResult> Handle(HttpContext ctx, CancellationToken ct)
    {
        var loggerFactory = ctx.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(JsonRpcHandler));
        
        try
        {
            var (errorResponse, rpcRequest) = await JsonRpcRequestParser.ParseAndValidate(ctx.Request.Body, ct);
            if (errorResponse != null)
            {
                return errorResponse;
            }

            if (rpcRequest == null)
            {
                return new JsonRpcResultError(RpcError.ParseError("Not request found"));
            }

            if (rpcRequest.Id == null)
            {
                return new JsonRpcResultError(
                    RpcError.ServerError(new NotImplementedException("Notifications are not implemented.")));
            }
        
            var rpcMethod = ctx.RequestServices.GetKeyedService<JsonRpcMethodBase>(rpcRequest.Method);
            if (rpcMethod == null)
            {
                return new JsonRpcResultError(RpcError.MethodNotFound());
            }
            
            var response = await rpcMethod.HandleInternal(rpcRequest, ct);
        
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle JSON-RPC request");
            return new JsonRpcResultError(RpcError.ServerError(ex));
        }
    }
}
