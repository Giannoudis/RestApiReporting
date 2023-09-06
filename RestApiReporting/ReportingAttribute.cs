namespace RestApiReporting;

/// <summary>Marker for included report assemblies, types and methods</summary>
[AttributeUsage(validOn:AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ReportingAttribute : Attribute
{
}
