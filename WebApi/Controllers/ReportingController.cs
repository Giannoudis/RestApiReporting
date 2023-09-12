using RestApiReporting.Service;
using RestApiReporting.WebApi.Reports;

namespace RestApiReporting.WebApi.Controllers;

// change the base routing
//[Route("myreports")]
public class ReportingController : ReportingControllerBase
{
    public ReportingController(IApiReportingService reportingService,
        ITenantMemoryReport tenantMemoryReport) :
        base(reportingService)
    {
        reportingService.Report.AddReportAsync(tenantMemoryReport);
    }
}