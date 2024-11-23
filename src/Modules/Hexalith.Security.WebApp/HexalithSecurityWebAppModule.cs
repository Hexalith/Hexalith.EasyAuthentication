namespace Hexalith.Security.WebApp;

using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;
using Hexalith.Extensions.Configuration;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application;
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
    public static string Path => HexalithSecurityApplicationInformation.ShortName;

    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => Name + " Module";

    /// <inheritdoc/>
    public string Id => $"{HexalithSecurityApplicationInformation.Id}.WebApp";

    /// <inheritdoc/>
    public string Name => $"{HexalithSecurityApplicationInformation.Name} Web App";

    /// <inheritdoc/>
    public int OrderWeight => 0;

    /// <inheritdoc/>
    string IApplicationModule.Path => Path;

    /// <inheritdoc/>
    public IEnumerable<Assembly> PresentationAssemblies => [GetType().Assembly, typeof(SecurityIndex).Assembly, typeof(ClaimsView).Assembly];

    /// <inheritdoc/>
    public string Version => field ??= this.ProductVersion() ?? "1.0";

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

        _ = services.AddAuthorizationCore();
        _ = services.AddScoped<AuthenticationStateProvider, WebAppPersistentAuthenticationStateProvider>();
        _ = services
            .AddCascadingAuthenticationState()
            .AddSingleton(p => SecurityMenu.Menu)
            .ConfigureSettings<SecuritySettings>(configuration);
    }

    /// <inheritdoc/>
    public void UseModule(object application)
    {
    }
}