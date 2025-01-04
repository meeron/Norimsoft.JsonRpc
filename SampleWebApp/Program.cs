using Norimsoft.JsonRpc;
using SampleWebApp.Features.Products.Methods;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddJsonRpc();
//services.AddJsonRpc(config => config.RegisterMethodsFrom(typeof(GetProduct).Assembly));

var app = builder.Build();

app.UseJsonRpc();

await app.RunAsync();
