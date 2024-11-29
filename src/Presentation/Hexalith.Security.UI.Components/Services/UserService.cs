namespace Hexalith.Security.UI.Components.Services;

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Hexalith.Application;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;
using Hexalith.Security.UI.Components.ViewModels;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Service for managing users.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserClaimStore<CustomUser> _claimStore;
    private readonly IUserCollectionService _userCollectionService;
    private readonly IUserStore<CustomUser> _userStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="userCollectionService">The user collection service.</param>
    /// <param name="userStore">The user store.</param>
    /// <param name="claimStore">The claim store.</param>
    public UserService(
        IUserCollectionService userCollectionService,
        IUserStore<CustomUser> userStore,
        IUserClaimStore<CustomUser> claimStore)
    {
        ArgumentNullException.ThrowIfNull(userCollectionService);
        ArgumentNullException.ThrowIfNull(userStore);
        ArgumentNullException.ThrowIfNull(claimStore);

        _userCollectionService = userCollectionService;
        _userStore = userStore;
        _claimStore = claimStore;
    }

    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of user summary view models.</returns>
    public async Task<IEnumerable<UserSummaryViewModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<string> ids = await _userCollectionService.AllAsync().ConfigureAwait(false);
        List<Task<UserSummaryViewModel?>> userTasks = [];
        foreach (string id in ids)
        {
            userTasks.Add(GetUserSummaryAsync(id, CancellationToken.None));
        }

        return (await Task.WhenAll(userTasks).ConfigureAwait(false)).OfType<UserSummaryViewModel>();
    }

    private async Task<UserSummaryViewModel?> GetUserSummaryAsync(string userId, CancellationToken cancellationToken)
    {
        CustomUser? user = await _userStore.FindByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user == null)
        {
            return null;
        }

        IList<Claim> claims = await _claimStore.GetClaimsAsync(user, cancellationToken).ConfigureAwait(false);
        return new UserSummaryViewModel(
            userId,
            user.UserName,
            user.Email,
            claims.Any(p => p.Type == ClaimTypes.Role && p.Value == ApplicationRoles.GlobalAdministrator));
    }
}