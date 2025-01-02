using Norimsoft.JsonRpc;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddJsonRpc();

var app = builder.Build();

app.UseJsonRpc();

await app.RunAsync();
