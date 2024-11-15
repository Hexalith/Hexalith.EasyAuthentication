namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.WebApp;

/// <summary>
/// Represents a client application.
/// </summary>
public class HexalithSecurityWebAppApplication : HexalithWebAppApplication
{
    /// <inheritdoc/>
    public override Type SharedAssetsApplicationType => typeof(HexalithSecuritySharedAssetsApplication);

    /// <inheritdoc/>
    public override IEnumerable<Type> WebAppModules
        => [typeof(HexalithSecurityClientModule)];
}