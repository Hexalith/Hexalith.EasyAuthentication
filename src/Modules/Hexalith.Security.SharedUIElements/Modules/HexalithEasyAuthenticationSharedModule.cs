namespace Hexalith.Security.SharedUIElements.Modules;

using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;
using Hexalith.Security.Shared.Resources.Modules;
using Hexalith.Security.SharedUIElements.Configurations;
using Hexalith.Extensions.Configuration;
using Hexalith.Extensions.Helpers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Microsoft Security shared module.
/// </summary>
public class HexalithSecuritySharedModule : ISharedUIElementsApplicationModule
{
    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => "Hexalith Security shared module";

    /// <inheritdoc/>
    public string Id => "Hexalith.Security.Shared";

    /// <inheritdoc/>
    public string Name => "Hexalith Security shared";

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

        _ = services
            .AddSingleton(p => SecurityMenu.Menu)
            .ConfigureSettings<SecuritySettings>(configuration);
    }

    /// <inheritdoc/>
    public void UseModule(object builder)
    {
    }
}