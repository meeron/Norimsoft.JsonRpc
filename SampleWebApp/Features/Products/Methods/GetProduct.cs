using Norimsoft.JsonRpc;
using SampleWebApp.Features.Products.Models;

namespace SampleWebApp.Features.Products.Methods;

public class GetProduct : JsonRpcMethod<GetProductParam>
{
    public override async Task<IJsonRpcResponse> Handle(GetProductParam param, CancellationToken ct)
    {
        await Task.CompletedTask;
        //return Ok(new Product());
        return Error(404,$"Product '{param.Id}' not found");
    }
}

public record GetProductParam(Guid Id);
