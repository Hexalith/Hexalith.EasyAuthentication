namespace Hexalith.Security.WebServer.Middlewares;

using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Hexalith.Application.Sessions.Helpers;
using Hexalith.Security.SharedAssets;
using Hexalith.Security.WebServer.Models;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Middleware that simulates Azure Container Apps authentication in development environment.
/// </summary>
public class DevelopmentAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevelopmentAuthenticationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public DevelopmentAuthenticationMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (
            !context.Request.Path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase) &&
            !context.Request.Path.StartsWithSegments("/v1.0", StringComparison.OrdinalIgnoreCase) &&
            !context.Request.Path.StartsWithSegments("/actors", StringComparison.OrdinalIgnoreCase) &&
            !context.Request.Path.StartsWithSegments(new PathString("/dapr"), StringComparison.OrdinalIgnoreCase))
        {
            // Simulate the Azure Container App authentication headers
            if (string.IsNullOrWhiteSpace(context.Request.Headers[SecurityConstants.ClientPrincipalIdHeader]))
            {
                context.Request.Headers[SecurityConstants.ClientPrincipalIdHeader] = "jdoe";
            }

            if (string.IsNullOrWhiteSpace(context.Request.Headers[SecurityConstants.ClientPrincipalNameHeader]))
            {
                context.Request.Headers[SecurityConstants.ClientPrincipalNameHeader] = "John Doe";
            }

            if (string.IsNullOrWhiteSpace(context.Request.Headers[SecurityConstants.ClientPrincipalIdentityProviderHeader]))
            {
                context.Request.Headers[SecurityConstants.ClientPrincipalIdentityProviderHeader] = "dev";
            }

            if (string.IsNullOrWhiteSpace(context.Request.Headers[SecurityConstants.ClientPrincipalHeader]))
            {
                ClientPrincipal clientPrincipal = new(
                    context.Request.Headers[SecurityConstants.ClientPrincipalIdentityProviderHeader].ToString(),
                    [
                        new ClientPrincipalClaim(ClaimTypes.NameIdentifier, context.Request.Headers[SecurityConstants.ClientPrincipalIdHeader].ToString()),
                        new ClientPrincipalClaim(ClaimTypes.Name, context.Request.Headers[SecurityConstants.ClientPrincipalNameHeader].ToString()),
                        new ClientPrincipalClaim(ClaimTypes.Email, "jdoe@hexalith.com"),
                        new ClientPrincipalClaim(SessionHelper.IdentityProviderClaimName, context.Request.Headers[SecurityConstants.ClientPrincipalIdentityProviderHeader].ToString()),
                    ],
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                context.Request.Headers[SecurityConstants.ClientPrincipalHeader] = Convert
                    .ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clientPrincipal)));
            }
        }

        await _next(context).ConfigureAwait(false);
    }
}