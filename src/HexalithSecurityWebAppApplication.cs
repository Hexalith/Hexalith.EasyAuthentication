namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application;
using Hexalith.Security.WebApp;
using Hexalith.UI.Components.Modules;

/// <summary>
/// Represents a client application.
/// </summary>
public class HexalithSecurityWebAppApplication : HexalithWebAppApplication
{
    private string? _version;

    /// <inheritdoc/>
    public override string Id => $"{HexalithSecurityApplicationInformation.Id}.{ApplicationType}";

    /// <inheritdoc/>
    public override string Name => $"{HexalithSecurityApplicationInformation.Name} {ApplicationType}";

    /// <inheritdoc/>
    public override string ShortName => HexalithSecurityApplicationInformation.ShortName;

    /// <inheritdoc/>
    public override string Version => _version ??= VersionHelper.EntryProductVersion() ?? base.Version;

    /// <inheritdoc/>
    public override IEnumerable<Type> WebAppModules
        => [
            typeof(HexalithUIComponentsWebAppModule),
            typeof(HexalithSecurityWebAppModule)];
}