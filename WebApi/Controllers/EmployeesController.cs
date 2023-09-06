using Microsoft.AspNetCore.Mvc;
using RestApiReporting.WebApi.Model;
using RestApiReporting.WebApi.Service;

namespace RestApiReporting.WebApi.Controllers;

[ApiController]
[Route("employees")]
public class EmployeesController : ControllerBase
{
    [HttpGet("Employees", Name = "GetEmployees")]
    //    [ApiOperationId("Employees")]
    public List<Employee> Get(string? tenantId = null)
    {
        // file based persistence
        var employees = new EmployeeService().GetEmployees();

        // tenant filter
        if (tenantId != null)
        {
            employees = employees
                .Where(x => string.Equals(x.TenantId, tenantId, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return employees;
    }

    [HttpGet("salary", Name = "GetSalaryEmployees")]
    public List<Employee> GetEmployeesBySalary(
        decimal? minSalary = 1000, decimal? maxSalary = null)
    {
        var employees = new EmployeeService().GetEmployees();

        // salary filter
        if (!minSalary.HasValue && !maxSalary.HasValue)
        {
            return employees.ToList();
        }

        minSalary ??= decimal.MinValue;
        maxSalary ??= decimal.MaxValue;
        employees = employees
            .Where(x => x.Salary >= minSalary && x.Salary <= maxSalary)
            .ToList();
        return employees;
    }
}