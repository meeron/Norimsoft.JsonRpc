namespace SampleWebApp.Features.Products.Models;

public class Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
