namespace Hexalith.Security.UI.Components.Users;

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Hexalith.Application;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.DaprIdentityStore.Services;

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

    /// <inheritdoc/>
    public Task AddGlobalAdministratorAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return _claimStore.AddClaimsAsync(
            new CustomUser { Id = userId },
            [new Claim(ClaimTypes.Role, ApplicationRoles.GlobalAdministrator)],
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        _ = await _userStore.DeleteAsync(new CustomUser { Id = userId }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DisableAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        CustomUser? user = await _userStore.FindByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user is null or { Disabled: true })
        {
            return;
        }

        user.Disabled = true;
        _ = await _userStore.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task EnableAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        CustomUser? user = await _userStore.FindByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user is null or { Disabled: false })
        {
            return;
        }

        user.Disabled = false;
        _ = await _userStore.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<UserDetailsViewModel?> FindAsync(string userId, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task<UserSummaryViewModel?> FindSummaryAsync(string userId, CancellationToken cancellationToken)
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
            user.Disabled,
            claims.Any(p => p.Type == ClaimTypes.Role && p.Value == ApplicationRoles.GlobalAdministrator));
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
            userTasks.Add(FindSummaryAsync(id, CancellationToken.None));
        }

        return (await Task.WhenAll(userTasks).ConfigureAwait(false))
            .OfType<UserSummaryViewModel>()
            .OrderBy(p => p.Name);
    }

    /// <inheritdoc/>
    public async Task<UserDetailsViewModel> GetAsync(string userId, CancellationToken cancellationToken)
        => await FindAsync(userId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

    /// <inheritdoc/>
    public async Task<UserSummaryViewModel> GetSummaryAsync(string userId, CancellationToken cancellationToken)
        => await FindSummaryAsync(userId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

    /// <inheritdoc/>
    public async Task GrantGlobalAdministratorRoleAsync(string userId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        await _claimStore.AddClaimsAsync(
            new CustomUser { Id = userId },
            [new Claim(ClaimTypes.Role, ApplicationRoles.GlobalAdministrator)],
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task RemoveGlobalAdministratorAsync(string userId, CancellationToken cancellationToken)
    {
        await _claimStore.RemoveClaimsAsync(
            new CustomUser { Id = userId },
            [new Claim(ClaimTypes.Role, ApplicationRoles.GlobalAdministrator)],
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task UpdateAsync(UserEditViewModel user, CancellationToken cancellationToken) => throw new NotImplementedException();
}