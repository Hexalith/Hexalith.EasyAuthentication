namespace Hexalith.Security.WebApp;

using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;
using Hexalith.Security.SharedAssets.Configurations;
using Hexalith.Extensions.Helpers;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Microsoft Easy Authentication client module.
/// </summary>
<<<<<<<< HEAD:src/Modules/Hexalith.Security.WebApp/HexalithEasyAuthenticationClientModule.cs
public class HexalithSecurityClientModule : IWebAppApplicationModule
========
public class HexalithEasyAuthenticationWebAppModule : IWebAppApplicationModule
>>>>>>>> 8e93b25a9298737142169f8ab68450fe81afe84a:src/Modules/Hexalith.Security.WebApp/HexalithEasyAuthenticationWebAppModule.cs
{
    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => "Microsoft Easy Authentication client module";

    /// <inheritdoc/>
    public string Id => "Hexalith.Security.Client";

    /// <inheritdoc/>
    public string Name => "Microsoft Easy Authentication client";

    /// <inheritdoc/>
    public int OrderWeight => 0;

    /// <inheritdoc/>
    public string Path => "Hexalith/Security";

    /// <inheritdoc/>
    public IEnumerable<Assembly> PresentationAssemblies => [GetType().Assembly];

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

        _ = services.AddScoped<AuthenticationStateProvider, ClientPersistentAuthenticationStateProvider>();
    }

    /// <inheritdoc/>
    public void UseModule(object builder)
    {
    }
}