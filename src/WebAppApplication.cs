namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.WebApp;

/// <summary>
/// Represents a client application.
/// </summary>
public class WebAppApplication : HexalithWebAppApplication
{
    /// <inheritdoc/>
    public override IEnumerable<Type> ClientModules
        => [typeof(HexalithSecurityClientModule)];

    /// <inheritdoc/>
    public override Type SharedAssetsApplicationType => typeof(SharedAssetsApplication);
}