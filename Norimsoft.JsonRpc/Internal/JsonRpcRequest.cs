using System.Text.Json;
using System.Text.Json.Serialization;

namespace Norimsoft.JsonRpc.Internal;

internal class JsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public JsonElement? Id { get; set; }
    public JsonElement? Params { get; set; }
}
