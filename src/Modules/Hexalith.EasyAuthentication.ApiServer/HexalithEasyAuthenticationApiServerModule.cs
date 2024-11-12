namespace Hexalith.EasyAuthentication.ApiServer;

using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Microsoft Easy Authentication server module.
/// </summary>
public sealed class HexalithEasyAuthenticationApiServerModule : IApiServerApplicationModule
{
    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => "Microsoft Easy Authentication API Server module";

    /// <inheritdoc/>
    public string Id => "Hexalith.EasyAuthentication.ApiServer";

    /// <inheritdoc/>
    public string Name => "Microsoft Easy Authentication API Server";

    /// <inheritdoc/>
    public int OrderWeight => 0;

    /// <inheritdoc/>
    string IApplicationModule.Path => HexalithEasyAuthenticationApiServerModule.Path;

    /// <inheritdoc/>
    public IEnumerable<Assembly> PresentationAssemblies => [GetType().Assembly];

    /// <inheritdoc/>
    public string Version => "1.0";

    private static string Path => "Hexalith/EasyAuthentication";

    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    /// <inheritdoc/>
    public void UseModule(object builder)
    {
    }
}