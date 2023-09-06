using System.Reflection;
using System.Text.Json;
using RestApiReporting.WebApi.Model;

namespace RestApiReporting.WebApi.Service;

public class ProductService
{
    private string FileName { get; }
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ProductService()
    {
        var fileName = "Data\\Products.json";

        // file is located on the executable folder
        var assembly = Assembly.GetEntryAssembly();
        var directory = assembly != null
            ? Path.GetDirectoryName(assembly.Location)
            : Path.GetDirectoryName(fileName);
        FileName = directory != null
            ? Path.Combine(directory, fileName)
            : fileName;
    }

    /// <summary>Get products</summary>
    public List<Product> GetProducts()
    {
        if (!File.Exists(FileName))
        {
            return new();
        }

        var caseFields = JsonSerializer.Deserialize<List<Product>>(
            File.ReadAllText(FileName),
            serializerOptions);
        return caseFields ?? new();
    }
}