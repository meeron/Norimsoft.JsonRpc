using Norimsoft.JsonRpc;
using SampleWebApp.Features.Products.Models;

namespace SampleWebApp.Features.Products.Methods;

public class GetProducts : JsonRpcMethod
{
    public override async Task<IJsonRpcResult> Handle(CancellationToken ct)
    {
        await Task.CompletedTask;
        Product[] products =
        [
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Apples",
                Price = 6.99M,
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Oranges",
                Price = 3.49M,
            },
        ];
        
        return Ok(products);
    }
}
