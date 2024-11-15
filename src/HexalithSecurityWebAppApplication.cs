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
    public override Type SharedUIElementsApplicationType => typeof(HexalithSecuritySharedUIElementsApplication);

    /// <inheritdoc/>
    public override IEnumerable<Type> WebAppModules
        => [typeof(HexalithSecurityWebAppModule)];
}