using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RestApiReporting.WebApi.Model;

namespace RestApiReporting.WebApi.Controllers;

[ApiController]
[Route("cultures")]
public class CulturesController : ControllerBase
{
    /// <summary>Get available cultures</summary>
    [HttpGet(Name = "GetCultures")]
    public IEnumerable<CultureDescription> GetCultures(int? skip, int? top)
    {
        var cultures = new List<CultureDescription>();
        // all cultures
        var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures)
                            .Where(x => (!string.IsNullOrWhiteSpace(x.Name)))
                            .ToList();

        // filter
        if (skip.HasValue)
        {
            cultureInfos = cultureInfos.Skip(skip.Value).ToList();
        }
        if (top.HasValue)
        {
            cultureInfos = cultureInfos.Take(top.Value).ToList();
        }

        // result
        foreach (var cultureInfo in cultureInfos)
        {
            cultures.Add(new CultureDescription(cultureInfo));
        }
        return cultures;
    }
}