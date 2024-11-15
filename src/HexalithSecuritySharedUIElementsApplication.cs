namespace Hexalith.Security.Client;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.SharedUIElements.Modules;
using Hexalith.UI.Components.Modules;

/// <summary>
/// Represents a shared application.
/// </summary>
public class HexalithSecuritySharedUIElementsApplication : HexalithSharedUIElementsApplication
{
    /// <inheritdoc/>
    public override string HomePath => "Security";

    /// <inheritdoc/>
    public override string Id => "Hexalith.Security.SharedUIElements";

    /// <inheritdoc/>
    public override string LoginPath => ".auth/login";

    /// <inheritdoc/>
    public override string LogoutPath => ".auth/logout";

    /// <inheritdoc/>
    public override string Name => "Hexalith Security Shared Assets";

    /// <inheritdoc/>
    public override IEnumerable<Type> SharedUIElementsModules =>
    [
        typeof(HexalithSecuritySharedModule), typeof(HexalithUIComponentsSharedModule)
    ];

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    public override string Version => "1.0";
}