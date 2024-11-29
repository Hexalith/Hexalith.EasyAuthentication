namespace Hexalith.Security.UI.Components.Roles;

/// <summary>
/// Represents a summary view model for a role.
/// </summary>
/// <param name="Id">The unique identifier of the role.</param>
/// <param name="Name">The name of the role.</param>
public record RoleSummaryViewModel(string Id, string? Name);