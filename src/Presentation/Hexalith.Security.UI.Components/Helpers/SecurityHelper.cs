namespace Hexalith.Security.UI.Components.Helpers;

using Hexalith.Security.UI.Components.Roles;
using Hexalith.Security.UI.Components.Users;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides helper methods for adding security UI components to the service collection.
/// </summary>
public static class SecurityHelper
{
    /// <summary>
    /// Adds the security UI components to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the components to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSecurityUIComponents(this IServiceCollection services)
    {
        _ = services.AddScoped<IUserService, UserService>();
        _ = services.AddScoped<IRoleService, RoleService>();
        return services;
    }
}