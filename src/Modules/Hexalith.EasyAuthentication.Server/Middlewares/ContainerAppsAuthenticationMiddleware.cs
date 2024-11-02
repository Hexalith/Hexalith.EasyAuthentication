namespace Hexalith.EasyAuthentication.Server.Middlewares;

using System.Security.Claims;

using Hexalith.EasyAuthentication.Shared;

using Microsoft.AspNetCore.Http;

/// <summary>
/// This middleware processes the following Container Apps authentication headers:
/// - X-MS-CLIENT-PRINCIPAL-NAME: The name of the authenticated user
/// - X-MS-CLIENT-PRINCIPAL-ID: The unique identifier of the authenticated user
/// - X-MS-CLIENT-PRINCIPAL-IDP: The identity provider used for authentication
/// - X-MS-CLIENT-PRINCIPAL: Base64 encoded JSON containing additional claims.
/// </summary>
public class ContainerAppsAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerAppsAuthenticationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public ContainerAppsAuthenticationMiddleware(RequestDelegate next)
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

        // Extract Container Apps authentication headers
        string principalName = context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalNameHeader].ToString();
        string principalId = context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdHeader].ToString();
        string principalIdp = context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdentityProviderHeader].ToString();

        if (string.IsNullOrWhiteSpace(principalId))
        {
            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, principalId),
            ];
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

        await _next(context).ConfigureAwait(false);
    }
}