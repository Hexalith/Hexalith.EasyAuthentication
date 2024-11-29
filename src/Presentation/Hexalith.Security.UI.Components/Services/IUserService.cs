namespace Hexalith.Security.UI.Components.Services;

using System.Threading.Tasks;

using Hexalith.Security.UI.Components.ViewModels;

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
}