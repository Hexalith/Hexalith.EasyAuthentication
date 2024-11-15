namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.ApiServer;

/// <summary>
/// Represents a server application.
/// </summary>
public class HexalithSecurityApiServerApplication : HexalithApiServerApplication
{
    /// <inheritdoc/>
    public override IEnumerable<Type> ApiServerModules => [typeof(HexalithSecurityApiServerModule)];

    /// <inheritdoc/>
    public override Type SharedUIElementsApplicationType => typeof(HexalithSecuritySharedUIElementsApplication);
}