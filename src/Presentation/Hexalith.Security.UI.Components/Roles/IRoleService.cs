namespace Hexalith.Security.UI.Components.Roles;

using System.Threading.Tasks;

/// <summary>
/// Defines the contract for user-related operations.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Retrieves all users asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of user summaries.</returns>
    Task<IEnumerable<RoleSummaryViewModel>> GetAllAsync(CancellationToken cancellationToken);
}