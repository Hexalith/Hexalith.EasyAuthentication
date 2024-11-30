namespace Hexalith.Security.UI.Components.Users;

/// <summary>
/// Represents a summary view model for a user.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="Disabled">Indicates whether the user is disabled.</param>
/// <param name="GlobalAdministrator">Indicates whether the user is a global administrator.</param>
public record UserSummaryViewModel(
    string Id,
    string? Name,
    string? Email,
    bool Disabled,
    bool GlobalAdministrator);