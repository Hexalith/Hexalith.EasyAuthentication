namespace Hexalith.Security.WebServer;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application;
using Hexalith.Security.WebApp;
using Hexalith.UI.Components.Modules;

/// <summary>
/// Represents a server application.
/// </summary>
public class HexalithSecurityWebServerApplication : HexalithWebServerApplication
{
    private static readonly string _version = string.Empty;

    /// <inheritdoc/>
    public override string Id => $"{HexalithSecurityApplicationInformation.Id}.{ApplicationType}";

    /// <inheritdoc/>
    public override string Name => $"{HexalithSecurityApplicationInformation.Name} {ApplicationType}";

    /// <inheritdoc/>
    public override string ShortName => HexalithSecurityApplicationInformation.ShortName;

    /// <inheritdoc/>
    public override string Version => string.IsNullOrWhiteSpace(_version)
        ? typeof(HexalithSecurityWebServerModule).GetAssemblyVersion()
        : _version;

    /// <inheritdoc/>
    public override Type WebAppApplicationType => typeof(HexalithSecurityWebAppApplication);

    /// <inheritdoc/>
    public override IEnumerable<Type> WebServerModules => [
        typeof(HexalithUIComponentsWebServerModule),
        typeof(HexalithSecurityWebServerModule)];
}