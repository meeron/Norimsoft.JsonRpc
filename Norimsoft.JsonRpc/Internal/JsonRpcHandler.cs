using Microsoft.AspNetCore.Http;

namespace Norimsoft.JsonRpc.Internal;

internal static class JsonRpcHandler
{
    public static IResult Handle()
    {
        return Results.Ok(new
        {
            jsonrpc = "2.0",
        });
    }
}
