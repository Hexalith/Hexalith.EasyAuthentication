namespace Hexalith.Security.UI.Components.Users;

using System.Threading.Tasks;

/// <summary>
/// Defines the contract for user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves all users asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of user summaries.</returns>
    Task<IEnumerable<UserSummaryViewModel>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Grants the global administrator role to a user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to grant the role to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task GrantGloablAdministratorRoleAsync(string userId, CancellationToken cancellationToken);
}