using System.Reflection;
using System.Text.Json;
using RestApiReporting.WebApi.Model;

namespace RestApiReporting.WebApi.Service;

public class TenantService : ITenantService
{
    private string FileName { get; }
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public TenantService()
    {
        var fileName = "Data\\Tenants.json";

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
    public List<Tenant> GetTenants()
    {
        if (!File.Exists(FileName))
        {
            return new();
        }

        var caseFields = JsonSerializer.Deserialize<List<Tenant>>(
            File.ReadAllText(FileName),
            serializerOptions);
        return caseFields ?? new();
    }
}