namespace Hexalith.Security.UI.Components.Menu;

using Hexalith.UI.Components;
using Hexalith.UI.Components.Icons;

using Labels = Hexalith.Security.UI.Components.Resources.Modules.SecurityMenu;

/// <summary>
/// Represents the Manhole menu.
/// </summary>
public static class SecurityMenu
{
    /// <summary>
    /// Gets the menu information.
    /// </summary>
    public static MenuItemInformation Menu => new(
                    Labels.SecurityMenuItem,
                    string.Empty,
                    new IconInformation("Shield", 20, IconStyle.Regular),
                    true,
                    0,
                    [
                        new MenuItemInformation(
                            Labels.ManageAccountMenuItem,
                            "Account/Manage",
                            new IconInformation("BuildingRetailShield", 20, IconStyle.Regular),
                            false,
                            10,
                            []),
                        new MenuItemInformation(
                            Labels.RegisterUserMenuItem,
                            "Account/Register",
                            new IconInformation("BuildingRetailShield", 20, IconStyle.Regular),
                            false,
                            10,
                            []),
                        new MenuItemInformation(
                            Labels.LoginMenuItem,
                            "Account/Login",
                            new IconInformation("ShieldCheckMark", 20, IconStyle.Regular),
                            false,
                            10,
                            []),
                        new MenuItemInformation(
                            Labels.LogoutMenuItem,
                            "Logout",
                            new IconInformation("ShieldDismiss", 20, IconStyle.Regular),
                            false,
                            10,
                            []),
                        new MenuItemInformation(
                            Labels.UserIdentityMenuItem,
                            "Security/UserIdentity",
                            new IconInformation("ShieldPerson", 20, IconStyle.Regular),
                            false,
                            10,
                            []),
                        new MenuItemInformation(
                            Labels.ClaimsMenuItem,
                            "/Security/Claim",
                            new IconInformation("ShieldKeyhole", 20, IconStyle.Regular),
                            false,
                            30,
                            []),
                    ]);
}