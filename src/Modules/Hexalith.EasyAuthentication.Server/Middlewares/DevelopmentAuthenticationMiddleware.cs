namespace Hexalith.EasyAuthentication.Server.Middlewares;

using Hexalith.EasyAuthentication.Shared;

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

        // Simulate the Azure Container App authentication headers
        if (string.IsNullOrWhiteSpace(context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdHeader]))
        {
            context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdHeader] = "jdoe@hexalith.com";
        }

        if (string.IsNullOrWhiteSpace(context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalNameHeader]))
        {
            context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalNameHeader] = "John Doe";
        }

        if (string.IsNullOrWhiteSpace(context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdentityProviderHeader]))
        {
            context.Request.Headers[EasyAuthenticationConstants.ClientPrincipalIdentityProviderHeader] = "dev";
        }

        await _next(context).ConfigureAwait(false);
    }
}