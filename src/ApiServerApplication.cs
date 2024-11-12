namespace Hexalith.EasyAuthentication.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.EasyAuthentication.ApiServer;

/// <summary>
/// Represents a server application.
/// </summary>
internal class ApiServerApplication : HexalithApiServerApplication
{
    /// <inheritdoc/>
    public override IEnumerable<Type> ApiServerModules => [typeof(HexalithEasyAuthenticationApiServerModule)];

    /// <inheritdoc/>
    public override Type SharedAssetsApplicationType => typeof(SharedAssetsApplication);
}