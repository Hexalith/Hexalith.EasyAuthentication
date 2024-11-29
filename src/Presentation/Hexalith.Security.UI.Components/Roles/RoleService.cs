namespace Hexalith.Security.UI.Components.Roles;

using System.Threading;
using System.Threading.Tasks;

using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Service for managing users.
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRoleCollectionService _userCollectionService;
    private readonly IRoleStore<CustomRole> _userStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleService"/> class.
    /// </summary>
    /// <param name="userCollectionService">The user collection service.</param>
    /// <param name="userStore">The user store.</param>
    /// <param name="claimStore">The claim store.</param>
    public RoleService(
        IRoleCollectionService userCollectionService,
        IRoleStore<CustomRole> userStore)
    {
        ArgumentNullException.ThrowIfNull(userCollectionService);
        ArgumentNullException.ThrowIfNull(userStore);

        _userCollectionService = userCollectionService;
        _userStore = userStore;
    }

    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of user summary view models.</returns>
    public async Task<IEnumerable<RoleSummaryViewModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<string> ids = await _userCollectionService.AllAsync().ConfigureAwait(false);
        List<Task<CustomRole?>> roleTasks = [];
        foreach (string id in ids)
        {
            roleTasks.Add(_userStore.FindByIdAsync(id, cancellationToken));
        }

        return (await Task.WhenAll(roleTasks).ConfigureAwait(false))
                .OfType<CustomRole>()
                .Select(p => new RoleSummaryViewModel(p.Id, p.Name))
                .OrderBy(p => p.Name);
    }
}