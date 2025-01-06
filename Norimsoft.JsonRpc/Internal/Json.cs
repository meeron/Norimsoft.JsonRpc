using System.Text.Json;

namespace Norimsoft.JsonRpc.Internal;

internal static class Json
{
    internal static string Serialize(object value) =>
        JsonSerializer.Serialize(value, JsonSerializerOptions.Web);
    
    internal static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, JsonSerializerOptions.Web)!;
}
