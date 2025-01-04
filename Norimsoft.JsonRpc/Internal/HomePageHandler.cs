using Microsoft.AspNetCore.Http;

namespace Norimsoft.JsonRpc.Internal;

internal static class HomePageHandler
{
    public static IResult Handle()
    {
        return Results.Text("Hello World!");
    }
}
