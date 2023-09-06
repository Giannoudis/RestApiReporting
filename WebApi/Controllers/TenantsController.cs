using Microsoft.AspNetCore.Mvc;
using RestApiReporting.WebApi.Model;
using RestApiReporting.WebApi.Service;

namespace RestApiReporting.WebApi.Controllers;

[ApiController]
[Route("tenants")]
public class TenantsController : ControllerBase
{
    [HttpGet(Name = "GetTenants")]
    public Task<List<Tenant>> Get()
    {
        var tenants = new TenantService().GetTenants();
        return Task.FromResult(tenants);
    }
}