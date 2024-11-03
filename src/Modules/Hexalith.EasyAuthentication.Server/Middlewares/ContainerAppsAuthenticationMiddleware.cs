namespace Hexalith.EasyAuthentication.Server.Middlewares;

using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Hexalith.Application.Modules.Applications;
using Hexalith.Application.Sessions;
using Hexalith.Application.Sessions.Helpers;
using Hexalith.Application.Sessions.Models;
using Hexalith.Application.Sessions.Services;
using Hexalith.EasyAuthentication.Shared;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// This middleware processes the following Container Apps authentication headers:
/// - X-MS-CLIENT-PRINCIPAL-NAME: The name of the authenticated user
/// - X-MS-CLIENT-PRINCIPAL-ID: The unique identifier of the authenticated user
/// - X-MS-CLIENT-PRINCIPAL-IDP: The identity provider used for authentication
/// - X-MS-CLIENT-PRINCIPAL: Base64 encoded JSON containing additional claims.
/// </summary>
public partial class ContainerAppsAuthenticationMiddleware
{
    private readonly IApplication _application;
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerAppsAuthenticationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="serviceProvider">The session service.</param>
    /// <param name="application">The application.</param>
    public ContainerAppsAuthenticationMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IApplication application)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(application);

        _next = next;
        _serviceProvider = serviceProvider;
        _application = application;
    }

    [LoggerMessage(
                EventId = 1,
                Level = LogLevel.Information,
                Message = "X-MS-PRINCIPAL: Id='{PrincipalId}' Name='{PrincipalName}' Idp='{PrincipalIdp}' Principal={Principal}")]
    public static partial void LogClientPrincipalInformation(ILogger logger, string principalId, string principalName, string principalIdp, string? principal);

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Request.Path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments(new PathString("/dapr"), StringComparison.OrdinalIgnoreCase))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        string? sessionId = context.Session.GetString(SessionConstants.SessionIdName);
        int? expiration = context.Session.GetInt32(SessionConstants.SessionExpirationName);

        // If the session is still valid and not expired, continue the request.
        if (context.User.Identity?.IsAuthenticated == true
            && !string.IsNullOrWhiteSpace(sessionId)
            && !expiration.HasExpired(TimeProvider.System.GetLocalNow()))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        // Extract principal identifier from Container Apps authentication headers
        string principalId = context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdHeader].ToString();
        string principalName = context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalNameHeader].ToString();
        string principalIdp = context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdentityProviderHeader].ToString();
        string? principal = context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalHeader].ToString();
        principal = string.IsNullOrWhiteSpace(principal) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(principal));

        using IServiceScope scope = _serviceProvider.CreateScope();
        ISessionService sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<ContainerAppsAuthenticationMiddleware>>();

        LogClientPrincipalInformation(logger, principalId, principalName, principalIdp, principal);

        if (string.IsNullOrWhiteSpace(principalId))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            Models.ClientPrincipal? headerPrincipal = string.IsNullOrWhiteSpace(principal)
                ? null
                : JsonSerializer.Deserialize<Models.ClientPrincipal>(principal);

            List<Claim> claims = [new(ClaimTypes.NameIdentifier, principalId)];
            if (headerPrincipal is not null)
            {
                claims.AddRange(headerPrincipal.Claims.Select(c => new Claim(c.Type, c.Value)));
            }

            if (!string.IsNullOrWhiteSpace(principalName))
            {
                claims.Add(new(ClaimTypes.Name, principalName));
            }

            if (!string.IsNullOrWhiteSpace(principalIdp))
            {
                claims.Add(new("idp", principalIdp));
            }

            ClaimsIdentity identity = new(claims, principalIdp);
            context.User = new ClaimsPrincipal(identity);
        }

        SessionInformation? session = string.IsNullOrWhiteSpace(sessionId)
                ? await sessionService
                    .OpenAsync(GetPartition(context), CancellationToken.None)
                    .ConfigureAwait(false)
                : await sessionService
                .GetAsync(sessionId, CancellationToken.None)
                .ConfigureAwait(false)
                ?? await sessionService
                    .OpenAsync(GetPartition(context), CancellationToken.None)
                    .ConfigureAwait(false);
        if (session is not null)
        {
            context.Session.SetString($"{nameof(Session)}{nameof(Session.Id)}", session.Id);
            context.Session.SetInt32($"{nameof(Session)}{nameof(Session.Expiration)}", session.CreatedOn.ExpirationInEpochMinutes(session.Expiration));
        }

        await _next(context).ConfigureAwait(false);
    }

    private static string? GetPartition(HttpContext context)
        => context.Request.Headers[EasyAuthenticationConstants.PartitionHeader].ToString()
            ?? context.Request.Query[$"{nameof(Session.PartitionId)}"].ToString()
            ?? context.Session.GetString($"{nameof(Session.PartitionId)}");
}