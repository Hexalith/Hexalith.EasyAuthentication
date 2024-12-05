namespace Hexalith.Security.WebServer.Controllers;

using Microsoft.Extensions.Logging;

/// <summary>
/// Provides logging functionality for the UserPartitionController.
/// </summary>
internal static partial class UserPartitionControllerLogger
{
    /// <summary>
    /// Logs a warning when the partition id is required and cannot be empty.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Partition id is required and cannot be empty.")]
    public static partial void LogPartitionIdRequired(this ILogger logger);

    /// <summary>
    /// Logs a warning when the user id is required and cannot be empty.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "User id is required and cannot be empty.")]
    public static partial void LogUserIdRequired(this ILogger logger);
}