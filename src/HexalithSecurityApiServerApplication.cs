namespace Hexalith.Security.ApiServer;

using System;
using System.Collections.Generic;

using Hexalith.Application.Modules.Applications;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application;

/// <summary>
/// Represents a server application.
/// </summary>
public class HexalithSecurityApiServerApplication : HexalithApiServerApplication
{
    private static string? _version;

    /// <inheritdoc/>
    public override IEnumerable<Type> ApiServerModules => [typeof(HexalithSecurityApiServerModule)];

    /// <inheritdoc/>
    public override string Id => $"{HexalithSecurityApplicationInformation.Id}.{ApplicationType}";

    /// <inheritdoc/>
    public override string Name => $"{HexalithSecurityApplicationInformation.Name} {ApplicationType}";

    /// <inheritdoc/>
    public override string ShortName => HexalithSecurityApplicationInformation.ShortName;

    /// <inheritdoc/>
    public override string Version => _version ??= typeof(HexalithSecurityApiServerModule).GetAssemblyVersion();
}