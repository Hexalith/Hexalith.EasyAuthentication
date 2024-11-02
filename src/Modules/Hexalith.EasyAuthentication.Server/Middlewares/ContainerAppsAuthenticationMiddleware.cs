namespace Hexalith.EasyAuthentication.Server.Middlewares;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>
/// Middleware to extract Container Apps authentication headers and create claims identity.
/// </summary>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="logger">The logger instance used to log information and errors during authentication processing.</param>
/// <remarks>
/// This middleware processes the following Container Apps authentication headers:
/// - X-MS-CLIENT-PRINCIPAL-NAME: The name of the authenticated user
/// - X-MS-CLIENT-PRINCIPAL-ID: The unique identifier of the authenticated user
/// - X-MS-CLIENT-PRINCIPAL-IDP: The identity provider used for authentication
/// - X-MS-CLIENT-PRINCIPAL: Base64 encoded JSON containing additional claims.
/// </remarks>
public partial class ContainerAppsAuthenticationMiddleware(
    RequestDelegate next,
    ILogger<ContainerAppsAuthenticationMiddleware> logger)
{
    /// <summary>
    /// Processes an HTTP request to extract authentication information from Container Apps headers
    /// and create a ClaimsPrincipal for the authenticated user.
    /// </summary>
    /// <param name="context">The HTTP context containing the request and response objects.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous middleware operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        LogStartingAuthentication(logger, context.Request.Path);

        // Extract Container Apps authentication headers
        string principalName = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].ToString();
        string principalId = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"].ToString();
        string principalIdp = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-IDP"].ToString();

        bool hasName = !string.IsNullOrEmpty(principalName);
        bool hasId = !string.IsNullOrEmpty(principalId);

        if (hasName && hasId)
        {
            // Create claims identity from Container Apps headers
            List<Claim> claims =
            [
                new(ClaimTypes.Name, principalName),
                new(ClaimTypes.NameIdentifier, principalId),
                new("idp", principalIdp),
            ];

            LogPrincipalInfo(logger, principalName, principalId, principalIdp);

            // Add additional claims from the base64 encoded X-MS-CLIENT-PRINCIPAL header
            string principalHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL"].ToString();
            if (!string.IsNullOrEmpty(principalHeader))
            {
                LogProcessingAdditionalClaims(logger);
                try
                {
                    byte[] decodedPrincipal = Convert.FromBase64String(principalHeader);
                    using MemoryStream memoryStream = new(decodedPrincipal);
                    using JsonDocument jsonDocument = await JsonDocument.ParseAsync(memoryStream).ConfigureAwait(false);
                    JsonElement claimsArray = jsonDocument.RootElement.GetProperty("claims");

                    foreach (JsonElement claim in claimsArray.EnumerateArray())
                    {
                        string? type = claim.GetProperty("typ").GetString();
                        string? value = claim.GetProperty("val").GetString();
                        if (type is not null && value is not null)
                        {
                            claims.Add(new(type, value));
                            LogClaimAdded(logger, type, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogPrincipalHeaderError(logger, ex);
                    throw; // Re-throw the exception to be handled further up the pipeline.
                }
            }

            ClaimsIdentity identity = new(claims, "ContainerApps");
            context.User = new ClaimsPrincipal(identity);
            LogAuthenticationSuccess(logger, principalName);
        }
        else
        {
            LogMissingPrincipal(logger, hasName, hasId);
        }

        await next(context).ConfigureAwait(false);
    }

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "ContainerAppsAuthenticationMiddleware: Authentication successful for user {PrincipalName}")]
    private static partial void LogAuthenticationSuccess(ILogger logger, string principalName);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Debug,
        Message = "ContainerAppsAuthenticationMiddleware: Added claim - Type: {ClaimType}, Value: {ClaimValue}")]
    private static partial void LogClaimAdded(ILogger logger, string claimType, string claimValue);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Warning,
        Message = "ContainerAppsAuthenticationMiddleware: Required authentication headers missing - Principal Name Present: {HasName}, Principal ID Present: {HasId}")]
    private static partial void LogMissingPrincipal(ILogger logger, bool hasName, bool hasId);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Error,
        Message = "ContainerAppsAuthenticationMiddleware: Error processing X-MS-CLIENT-PRINCIPAL header")]
    private static partial void LogPrincipalHeaderError(ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "ContainerAppsAuthenticationMiddleware: Principal information - Name: {PrincipalName}, ID: {PrincipalId}, Identity Provider: {PrincipalIdp}")]
    private static partial void LogPrincipalInfo(ILogger logger, string principalName, string principalId, string principalIdp);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Debug,
        Message = "ContainerAppsAuthenticationMiddleware: Processing additional claims from X-MS-CLIENT-PRINCIPAL header")]
    private static partial void LogProcessingAdditionalClaims(ILogger logger);

    [LoggerMessage(
                                    EventId = 1000,
        Level = LogLevel.Information,
        Message = "ContainerAppsAuthenticationMiddleware: Starting authentication process for request path: {Path}")]
    private static partial void LogStartingAuthentication(ILogger logger, string path);
}