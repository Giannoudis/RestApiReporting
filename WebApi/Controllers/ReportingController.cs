using RestApiReporting.Service;

namespace RestApiReporting.WebApi.Controllers;

// change the base routing
//[Route("myreports")]
public class ReportingController : ReportingControllerBase
{
    public ReportingController(IApiReportingService reportingService) :
        base(reportingService)
    {
    }
}