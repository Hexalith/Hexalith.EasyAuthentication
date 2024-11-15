namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.WebServer;

/// <summary>
/// Represents a server application.
/// </summary>
public class HexalithSecurityWebServerApplication : HexalithWebServerApplication
{
    /// <inheritdoc/>
    public override Type SharedUIElementsApplicationType => typeof(HexalithSecuritySharedUIElementsApplication);

    /// <inheritdoc/>
    public override Type WebAppApplicationType => typeof(HexalithSecurityWebAppApplication);

    /// <inheritdoc/>
    public override IEnumerable<Type> WebServerModules => [typeof(HexalithSecurityServerModule)];
}