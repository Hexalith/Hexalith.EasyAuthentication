namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.Application;
using Hexalith.Security.WebApp;
using Hexalith.UI.Components.Modules;

/// <summary>
/// Represents a client application.
/// </summary>
public class HexalithSecurityWebAppApplication : HexalithWebAppApplication
{
    /// <inheritdoc/>
    public override string Id => $"{HexalithSecurityApplicationInformation.Id}.{ApplicationType}";

    /// <inheritdoc/>
    public override string Name => $"{HexalithSecurityApplicationInformation.Name} {ApplicationType}";

    /// <inheritdoc/>
    public override string ShortName => HexalithSecurityApplicationInformation.ShortName;

    /// <inheritdoc/>
    public override IEnumerable<Type> WebAppModules
        => [
            typeof(HexalithUIComponentsWebAppModule),
            typeof(HexalithSecurityWebAppModule)];
}