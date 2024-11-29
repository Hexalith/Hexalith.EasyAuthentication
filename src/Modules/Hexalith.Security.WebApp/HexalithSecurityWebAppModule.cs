namespace Hexalith.Security.WebApp;

using System;
using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application;
using Hexalith.Security.Application.Configurations;
using Hexalith.Security.Application.Menu;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Microsoft Security client module.
/// </summary>
public sealed class HexalithSecurityWebAppModule : IWebAppApplicationModule
{
    private static string? _version;

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
    public IEnumerable<Assembly> PresentationAssemblies => [GetType().Assembly];

    /// <inheritdoc/>
    public string Version => _version ??= GetType().GetAssemblyVersion();

    private static string Path => HexalithSecurityApplicationInformation.ShortName;

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

        _ = services.AddSingleton(p => SecurityMenu.Menu);
    }

    /// <inheritdoc/>
    public void UseModule(object application)
    {
    }
}