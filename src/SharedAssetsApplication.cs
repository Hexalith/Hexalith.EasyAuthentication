namespace Hexalith.EasyAuthentication.Client;

using Hexalith.Application.Modules.Applications;
using Hexalith.EasyAuthentication.SharedAssets.Modules;
using Hexalith.UI.Components.Modules;

/// <summary>
/// Represents a shared application.
/// </summary>
public class SharedAssetsApplication : HexalithSharedAssetsApplication
{
    /// <inheritdoc/>
    public override string HomePath => "EasyAuthentication";

    /// <inheritdoc/>
    public override string Id => "HexalithEasyAuthentication";

    /// <inheritdoc/>
    public override string LoginPath => ".auth/login";

    /// <inheritdoc/>
    public override string LogoutPath => ".auth/logout";

    /// <inheritdoc/>
    public override string Name => "Hexalith Easy Authentication Shared Assets";

    /// <inheritdoc/>
    public override IEnumerable<Type> SharedModules =>
    [
        typeof(HexalithEasyAuthenticationSharedModule), typeof(HexalithUIComponentsSharedModule)
    ];

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    public override string Version => "1.0";
}