using RestApiReporting.WebApi.Model;

namespace RestApiReporting.WebApi.Service;

public interface ITenantService
{
    /// <summary>Get products</summary>
    public List<Tenant> GetTenants();
}