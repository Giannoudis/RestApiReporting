
namespace RestApiReporting;

/// <summary>Marker for excluded report assemblies, types and methods</summary>
[AttributeUsage(validOn: AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ReportingIgnoreAttribute : Attribute
{
}
