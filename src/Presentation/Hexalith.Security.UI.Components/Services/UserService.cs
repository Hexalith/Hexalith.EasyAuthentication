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
    private readonly IUserClaimStore<CustomUserClaim> _claimStore;
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
        IUserClaimStore<CustomUserClaim> claimStore)
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
        List<CustomUser> users = [];
        List<Task<CustomUser?>> userTasks = [];
        List<Task<CustomUserClaim?>> claimTasks = [];
        foreach (string id in ids)
        {
            userTasks.Add(_userStore.FindByIdAsync(id, CancellationToken.None));
            claimTasks.Add(_claimStore.FindByIdAsync(id, CancellationToken.None));
        }

        CustomUser?[] customUsers = await Task.WhenAll(userTasks).ConfigureAwait(false);
        CustomUserClaim?[] customUserClaims = await Task.WhenAll(claimTasks).ConfigureAwait(false);
        return customUsers
            .OfType<CustomUser>()
            .Select(p => new UserSummaryViewModel(
                    p.Id,
                    p.UserName,
                    p.Email,
                    customUserClaims.Where(p => p.ClaimType == ClaimTypes.Role && p.ClaimValue == ApplicationRoles.GlobalAdministrator).Any()));
    }
}