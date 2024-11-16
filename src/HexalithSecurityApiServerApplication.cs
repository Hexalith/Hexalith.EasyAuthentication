namespace Hexalith.Security.Client;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.ApiServer;
using Hexalith.Security.Application;

/// <summary>
/// Represents a server application.
/// </summary>
public class HexalithSecurityApiServerApplication : HexalithApiServerApplication
{
    /// <inheritdoc/>
    public override IEnumerable<Type> ApiServerModules => [typeof(HexalithSecurityApiServerModule)];

    /// <inheritdoc/>
    public override string Id => $"{HexalithSecurityApplicationInformation.Id}.{ApplicationType}";

    /// <inheritdoc/>
    public override string Name => $"{HexalithSecurityApplicationInformation.Name} {ApplicationType}";

    /// <inheritdoc/>
    public override string ShortName => HexalithSecurityApplicationInformation.ShortName;
}