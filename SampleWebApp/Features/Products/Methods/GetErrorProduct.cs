using Norimsoft.JsonRpc;

namespace SampleWebApp.Features.Products.Methods;

[JsonRpcMethodName("products.getError")]
public class GetErrorProduct : JsonRpcMethod
{
    public override Task<IJsonRpcResponse> Handle(CancellationToken ct)
    {
        throw new Exception("This is not the product you are trying to get.");
    }
}
