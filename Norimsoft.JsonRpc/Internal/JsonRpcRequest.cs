using System.Text.Json;
using System.Text.Json.Serialization;

namespace Norimsoft.JsonRpc.Internal;

internal class JsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = string.Empty;
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;
    [JsonPropertyName("id")]
    public JsonElement? Id { get; set; }
    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}
