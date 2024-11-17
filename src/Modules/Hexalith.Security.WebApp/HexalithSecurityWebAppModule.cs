namespace Hexalith.Security.WebApp;

using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;
using Hexalith.Extensions.Configuration;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application.Configurations;
using Hexalith.Security.UI.Components.Claims;
using Hexalith.Security.UI.Components.Menu;
using Hexalith.Security.UI.Pages.Security;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Microsoft Security client module.
/// </summary>
public class HexalithSecurityWebAppModule : IWebAppApplicationModule
{
    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => "Microsoft Security client module";

    /// <inheritdoc/>
    public string Id => "Hexalith.Security.Client";

    /// <inheritdoc/>
    public string Name => "Microsoft Security client";

    /// <inheritdoc/>
    public int OrderWeight => 0;

    /// <inheritdoc/>
    public string Path => "Hexalith/Security";

    /// <inheritdoc/>
    public IEnumerable<Assembly> PresentationAssemblies => [GetType().Assembly, typeof(SecurityIndex).Assembly, typeof(ClaimsView).Assembly];

    /// <inheritdoc/>
    public string Version => "1.0";

    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        SecuritySettings settings = configuration.GetSettings<SecuritySettings>()
            ?? throw new InvalidOperationException($"Could not load settings section '{SecuritySettings.ConfigurationName()}'");
        if (!settings.Enabled)
        {
            return;
        }

        _ = services.AddScoped<AuthenticationStateProvider, WebAppPersistentAuthenticationStateProvider>();
        _ = services
            .AddSingleton(p => SecurityMenu.Menu)
            .ConfigureSettings<SecuritySettings>(configuration);
    }

    /// <inheritdoc/>
    public void UseModule(object builder)
    {
    }
}