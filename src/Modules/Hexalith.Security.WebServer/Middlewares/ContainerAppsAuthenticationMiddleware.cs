namespace Hexalith.Security.Server.Middlewares;

using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Hexalith.Application.Modules.Applications;
using Hexalith.Application.Partitions.Services;
using Hexalith.Application.Sessions;
using Hexalith.Application.Sessions.Helpers;
using Hexalith.Application.Sessions.Models;
using Hexalith.Application.Sessions.Services;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application;
using Hexalith.Security.WebServer.Models;

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
    private readonly IPartitionService _partitionService;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerAppsAuthenticationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="serviceProvider">The session service.</param>
    /// <param name="application">The application.</param>
    public ContainerAppsAuthenticationMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IApplication application, IPartitionService partitionService)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(partitionService);

        _next = next;
        _serviceProvider = serviceProvider;
        _application = application;
        _partitionService = partitionService;
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
            context.Request.Path.StartsWithSegments("/actors", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/v1.0", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/v1.0-beta1", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/v1.0-alpha1", StringComparison.OrdinalIgnoreCase) ||
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

        using IServiceScope scope = _serviceProvider.CreateScope();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<ContainerAppsAuthenticationMiddleware>>();

        context.User = GetUser(context.User, context, logger);

        string partitionId = await GetPartitionAsync(context).ConfigureAwait(false);

        ISessionService sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();

        SessionInformation? session = await sessionService.OpenAsync(partitionId, CancellationToken.None).ConfigureAwait(false);
        if (session is not null)
        {
            context.Session.SetString($"{nameof(Session)}{nameof(Session.Id)}", session.Id);
            context.Session.SetInt32($"{nameof(Session)}{nameof(Session.Expiration)}", session.CreatedOn.ExpirationInEpochMinutes(session.Expiration));
            context.User = AddHexalithIdentity(context.User, session);
        }

        await _next(context).ConfigureAwait(false);
    }

    private static ClaimsPrincipal AddHexalithIdentity(ClaimsPrincipal user, SessionInformation sessionInformation)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return user;
        }

        ClaimsIdentity? hexalith = user.Identities.FirstOrDefault(i => i.AuthenticationType == SessionHelper.HexalithAuthenticationType);
        if (hexalith?.Claims.Any(
            p => p.Type == SessionHelper.SessionIdClaimName &&
            p.Value == sessionInformation.Id) == true)
        {
            return user;
        }

        ClaimsPrincipal newUser = new(user.Identity);
        foreach (ClaimsIdentity identity in user.Identities.Where(
            p => p.AuthenticationType != SessionHelper.HexalithAuthenticationType && p != user.Identity))
        {
            newUser.AddIdentity(identity);
        }

        newUser.AddIdentity(new ClaimsIdentity(
            sessionInformation
                .User
                .Roles
                .Select(p => new Claim(ClaimTypes.Role, p, null, SessionHelper.HexalithIssuerName))
            .Union(
                [

                new Claim(SessionHelper.SessionIdClaimName, sessionInformation.Id, null, SessionHelper.HexalithIssuerName),
                new Claim(SessionHelper.PartitionIdClaimName, sessionInformation.PartitionId, null, SessionHelper.HexalithIssuerName),
                new Claim(ClaimTypes.NameIdentifier, sessionInformation.User.Id, null, SessionHelper.HexalithIssuerName),
                new Claim(ClaimTypes.Name, sessionInformation.Contact.Name, null, SessionHelper.HexalithIssuerName),
                new Claim(ClaimTypes.Actor, sessionInformation.Contact.Id, null, SessionHelper.HexalithIssuerName),
                new Claim(ClaimTypes.Email, sessionInformation.Contact.Email),
                new Claim(ClaimTypes.Expiration, sessionInformation.CreatedOn.ExpirationInEpochMinutes(sessionInformation.Expiration).ToInvariantString())
            ]),
            SessionHelper.HexalithAuthenticationType));
        return user;
    }

    private static ClaimsPrincipal GetUser(ClaimsPrincipal? user, HttpContext context, ILogger logger)
    {
        if (user?.Identity?.IsAuthenticated == true)
        {
            return user;
        }

        // Extract principal identifier from Container Apps authentication headers
        string principalId = context.Request.Headers[SecurityConstants.ClientPrincipalIdHeader].ToString();
        string principalName = context.Request.Headers[SecurityConstants.ClientPrincipalNameHeader].ToString();
        string principalIdp = context.Request.Headers[SecurityConstants.ClientPrincipalIdentityProviderHeader].ToString();
        string? principal = context.Request.Headers[SecurityConstants.ClientPrincipalHeader].ToString();
        principal = string.IsNullOrWhiteSpace(principal) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(principal));

        LogClientPrincipalInformation(logger, principalId, principalName, principalIdp, principal);

        ClientPrincipal? clientPrincipal;
        ClaimsIdentity identity;
        if (!string.IsNullOrWhiteSpace(principal) && (clientPrincipal = JsonSerializer.Deserialize<ClientPrincipal>(principal)) != null)
        {
            identity = new(clientPrincipal.IdentityProvider, clientPrincipal.NameClaimType, clientPrincipal.RoleClaimType);
            identity.AddClaims(clientPrincipal.Claims.Select(c => new Claim(c.Type, c.Value)));
        }
        else
        {
            identity = new(principalIdp);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principalId));
            identity.AddClaim(new Claim(ClaimTypes.Name, principalName));
            identity.AddClaim(new Claim(SessionHelper.IdentityProviderClaimName, principalIdp));
        }

        return new(identity);
    }

    private async Task<string> GetPartitionAsync(HttpContext context)
    {
        string? partitionId = context.Request.Headers[SecurityConstants.PartitionHeader].ToString();
        if (!string.IsNullOrWhiteSpace(partitionId))
        {
            return partitionId;
        }

        partitionId = context.Request.Query[$"{nameof(Session.PartitionId)}"].ToString();
        if (!string.IsNullOrWhiteSpace(partitionId))
        {
            return partitionId;
        }

        partitionId = context.Session.GetString($"{nameof(Session.PartitionId)}");
        return !string.IsNullOrWhiteSpace(partitionId) ? partitionId : await _partitionService.DefaultAsync(CancellationToken.None).ConfigureAwait(false);
    }
}