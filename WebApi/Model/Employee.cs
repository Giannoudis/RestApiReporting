
namespace RestApiReporting.WebApi.Model;

public class Employee
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? EntryDate { get; set; }
    public decimal? Salary { get; set; }
}