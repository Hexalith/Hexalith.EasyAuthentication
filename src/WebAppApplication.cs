namespace Hexalith.EasyAuthentication.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.EasyAuthentication.WebApp;

/// <summary>
/// Represents a client application.
/// </summary>
public class WebAppApplication : HexalithWebAppApplication
{
    /// <inheritdoc/>
    public override IEnumerable<Type> ClientModules
        => [typeof(HexalithEasyAuthenticationClientModule)];

    /// <inheritdoc/>
    public override Type SharedAssetsApplicationType => typeof(SharedAssetsApplication);
}