namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.WebServer;

/// <summary>
/// Represents a server application.
/// </summary>
internal class WebServerApplication : HexalithWebServerApplication
{
    /// <inheritdoc/>
    public override Type SharedAssetsApplicationType => typeof(SharedAssetsApplication);

    /// <inheritdoc/>
    public override Type WebAppApplicationType => typeof(WebAppApplication);

    /// <inheritdoc/>
    public override IEnumerable<Type> WebServerModules => [typeof(HexalithSecurityServerModule)];
}