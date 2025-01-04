using Microsoft.AspNetCore.Builder;
using Norimsoft.JsonRpc.Internal;

namespace Norimsoft.JsonRpc;

public static class WebApplicationExtensions
{
    public static void UseJsonRpc(this WebApplication app)
    {
        app.MapGet("/", HomePageHandler.Handle);
        app.MapPost("/", JsonRpcHandler.Handle);
    }
}
