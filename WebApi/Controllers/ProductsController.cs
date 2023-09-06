using Microsoft.AspNetCore.Mvc;
using RestApiReporting.WebApi.Model;
using RestApiReporting.WebApi.Service;

namespace RestApiReporting.WebApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    /// <summary>Get all products</summary>
    [HttpGet(Name = "GetProducts")]
    public List<Product> GetProducts() =>
        new ProductService().GetProducts();

    /// <summary>Get all products async</summary>
    [HttpGet("async", Name = "GetProductsAsync")]
    public Task<List<Product>> GetProductsAsync() =>
        Task.FromResult(new ProductService().GetProducts());

    /// <summary>Get product</summary>
    [HttpGet("{name}", Name = "GetProduct")]
    public Product? GetProduct(string name)
    {
        var product = new ProductService().GetProducts()
            .FirstOrDefault(x => string.Equals(x.Name, name));
        return product;
    }

    /// <summary>Get product async</summary>
    [HttpGet("async/{name}", Name = "GetProductAsync")]
    public Task<Product?> GetProductAsync(string name)
    {
        var product = new ProductService().GetProducts()
            .FirstOrDefault(x => string.Equals(x.Name, name));
        return Task.FromResult(product);
    }
}