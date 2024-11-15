namespace Hexalith.Security.Client;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.SharedAssets.Modules;
using Hexalith.UI.Components.Modules;

/// <summary>
/// Represents a shared application.
/// </summary>
public class HexalithSecuritySharedAssetsApplication : HexalithSharedAssetsApplication
{
    /// <inheritdoc/>
    public override string HomePath => "Security";

    /// <inheritdoc/>
    public override string Id => "Hexalith.Security.SharedAssets";

    /// <inheritdoc/>
    public override string LoginPath => ".auth/login";

    /// <inheritdoc/>
    public override string LogoutPath => ".auth/logout";

    /// <inheritdoc/>
    public override string Name => "Hexalith Security Shared Assets";

    /// <inheritdoc/>
    public override IEnumerable<Type> SharedAssetsModules =>
    [
        typeof(HexalithSecuritySharedModule), typeof(HexalithUIComponentsSharedModule)
    ];

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    public override string Version => "1.0";
}