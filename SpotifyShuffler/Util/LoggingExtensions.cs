namespace booleancoercion.SpotifyShuffler.Util;

using Microsoft.Extensions.Logging;

public static class LoggingExtensions
{
    private static string Format(TraceId traceId, string message) => $"{traceId}: {message}";

    public static void LogTrace(this ILogger logger, TraceId traceId, string message)
    {
        logger.LogTrace(Format(traceId, message));
    }

    public static void LogTrace(this ILogger logger, Exception ex, TraceId traceId, string message)
    {
        logger.LogTrace(ex, Format(traceId, message));
    }

    public static void LogDebug(this ILogger logger, TraceId traceId, string message)
    {
        logger.LogDebug(Format(traceId, message));
    }

    public static void LogDebug(this ILogger logger, Exception ex, TraceId traceId, string message)
    {
        logger.LogDebug(ex, Format(traceId, message));
    }

    public static void LogInformation(this ILogger logger, TraceId traceId, string message)
    {
        logger.LogInformation(Format(traceId, message));
    }

    public static void LogInformation(this ILogger logger, Exception ex, TraceId traceId, string message)
    {
        logger.LogInformation(ex, Format(traceId, message));
    }

    public static void LogWarning(this ILogger logger, TraceId traceId, string message)
    {
        logger.LogWarning(Format(traceId, message));
    }

    public static void LogWarning(this ILogger logger, Exception ex, TraceId traceId, string message)
    {
        logger.LogWarning(ex, Format(traceId, message));
    }

    public static void LogError(this ILogger logger, TraceId traceId, string message)
    {
        logger.LogError(Format(traceId, message));
    }

    public static void LogError(this ILogger logger, Exception ex, TraceId traceId, string message)
    {
        logger.LogError(ex, Format(traceId, message));
    }

    public static void LogCritical(this ILogger logger, TraceId traceId, string message)
    {
        logger.LogCritical(Format(traceId, message));
    }

    public static void LogCritical(this ILogger logger, Exception ex, TraceId traceId, string message)
    {
        logger.LogCritical(ex, Format(traceId, message));
    }
}
