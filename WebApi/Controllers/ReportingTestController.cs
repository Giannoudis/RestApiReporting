using Microsoft.AspNetCore.Mvc;
using RestApiReporting.WebApi.Model;

namespace RestApiReporting.WebApi.Controllers;

[Route("reportingtest")]
[ApiController]
public class ReportingTestController : ControllerBase
{
    /// <summary>Get all products async</summary>
    [HttpGet("tests/types", Name = "GetDotNetTypes")]
    public Task<List<DotNetTypes>> GetDotNetTypesAsync(
        [FromQuery] string? stringValue,
        [FromQuery] DateTime? dateTimeValue,
        [FromQuery] char? charValue,
        [FromQuery] bool? booleanValue,
        [FromQuery] int? intValue,
        [FromQuery] long? longValue,
        [FromQuery] float? floatValue,
        [FromQuery] double? doubleValue,
        [FromQuery] decimal? decimalValue,
        [FromQuery] byte? byteValue,
        [FromQuery] sbyte? sbyteValue,
        [FromQuery] short? shortValue,
        [FromQuery] ushort? ushortValue,
        [FromQuery] uint? uintValue,
        [FromQuery] ulong? ulongValue) =>
        Task.FromResult(GetDotNetTypes());

    /// <summary>Get products</summary>
    private List<DotNetTypes> GetDotNetTypes()
    {
        var dotNetTypes = new List<DotNetTypes>();

        for (var i = 1; i <= 10; i++)
        {
            dotNetTypes.Add(new DotNetTypes
            {
                StringValue = $"Value {i}",
                DateTimeValue = DateTime.Now.Date.AddDays(i),
                CharValue = (char)(i + 32),
                BooleanValue = (i % 2) == 1,
                IntValue = i,
                LongValue = i,
                FloatValue = i,
                DoubleValue = i,
                DecimalValue = i,
                ByteValue = (byte)i,
                ShortValue = (short)i,
                SbyteValue = (sbyte)i,
                UshortValue = (ushort)i,
                UintValue = (uint)i,
                UlongValue = (ulong)i,
            });
        }
        return dotNetTypes;
    }
}