using System.Runtime.Serialization;

namespace RestApiReporting;

/// <summary>Report exception</summary>
public class ReportException : Exception
{
    /// <inheritdoc/>
    public ReportException()
    {
    }

    /// <inheritdoc/>
    public ReportException(string message) :
        base(message)
    {
    }

    /// <inheritdoc/>
    public ReportException(string message, Exception innerException) :
        base(message, innerException)
    {
    }

    /// <inheritdoc/>
    protected ReportException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}