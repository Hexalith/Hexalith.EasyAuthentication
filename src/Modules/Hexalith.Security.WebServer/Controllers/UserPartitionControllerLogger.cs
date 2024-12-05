namespace Hexalith.Security.WebServer.Controllers;

using Microsoft.Extensions.Logging;

/// <summary>
/// Provides logging functionality for the UserPartitionController.
/// </summary>
internal static partial class UserPartitionControllerLogger
{
    /// <summary>
    /// Logs a warning when the user id is required and cannot be empty.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userId">The user identifier.</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Default partition not found for user {userId}.")]
    public static partial void LogDefaultPartitionNotFound(this ILogger logger, string userId);

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

    /// <summary>
    /// Logs a warning when no partition is found for the user and a new partition is created.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userId">The user identifier.</param>
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "No partition found for user {userId}. Creating new partition.")]
    public static partial void NoPartitionFoundCreatingNewPartition(this ILogger logger, string userId);
}