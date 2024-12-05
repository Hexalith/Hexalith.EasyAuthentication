namespace Hexalith.Security.WebServer.Controllers;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Hexalith.Application.Partitions.Services;
using Hexalith.Application.Sessions.Services;
using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/// <summary>
/// Controller for managing user partitions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class UserPartitionController : ControllerBase
{
    private readonly ILogger<UserPartitionController> _logger;
    private readonly IPartitionService _partitionService;
    private readonly UserManager<CustomUser> _userManager;
    private readonly IUserPartitionService _userPartitionService;
    private readonly IUserStore<CustomUser> _userStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPartitionController"/> class.
    /// </summary>
    /// <param name="userPartitionService">The user partition service.</param>
    /// <param name="partitionService">The partition service.</param>
    /// <param name="userStore">The user store.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
    public UserPartitionController(
        IUserPartitionService userPartitionService,
        IPartitionService partitionService,
        IUserStore<CustomUser> userStore,
        UserManager<CustomUser> userManager,
        ILogger<UserPartitionController> logger)
    {
        ArgumentNullException.ThrowIfNull(userPartitionService);
        ArgumentNullException.ThrowIfNull(partitionService);
        ArgumentNullException.ThrowIfNull(userStore);
        _userPartitionService = userPartitionService ?? throw new ArgumentNullException(nameof(userPartitionService));
        _partitionService = partitionService;
        _userStore = userStore;
        _userManager = userManager;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the default partition for a user.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The default partition.</returns>
    [HttpGet("{userName}/default")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> GetDefaultPartitionAsync(
        [Required][FromRoute] string userName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            _logger.LogUserIdRequired();
            return BadRequest("User id is required and cannot be empty.");
        }

        string? result = await _userPartitionService
            .GetDefaultPartitionAsync(userName, cancellationToken)
            .ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(userName))
        {
            return Ok(result);
        }

        _logger.LogDefaultPartitionNotFound(userName);

        CustomUser? user = await _userStore.FindByNameAsync(_userManager.NormalizeName(userName), cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return BadRequest($"User {userName} not found.");
        }

        // When the default partition is not found, we try to find the first partition in the list of user partitions.
        result = user.Partitions.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(result))
        {
            // Set user default partition to the first user partition found.
            user.DefaultPartition = result;
            _ = await _userStore.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        // The user has no partition. We try to find the default partition in the system.
        result = await _partitionService
            .DefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        // Update the user with the default partition.
        user.DefaultPartition = result;
        user.Partitions = [result];
        _ = await _userStore.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Gets all partitions for a user.
    /// </summary>
    /// <param name="userName">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of partitions.</returns>
    [HttpGet("{userName}/partitions")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<string>>> GetPartitionsAsync(
        [Required][FromRoute] string userName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            _logger.LogUserIdRequired();
            return BadRequest("User id is required and cannot be empty.");
        }

        IEnumerable<string> result = await _userPartitionService
            .GetPartitionsAsync(_userManager.NormalizeName(userName), cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    /// Checks if a user is in a specific partition.
    /// </summary>
    /// <param name="userName">The user identifier.</param>
    /// <param name="partitionId">The partition identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the user is in the partition, otherwise false.</returns>
    [HttpGet("{userName}/in-partition/{partitionId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> InPartitionAsync(
        [Required][FromRoute] string userName,
        [Required][FromRoute] string partitionId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            _logger.LogUserIdRequired();
            return BadRequest("User id is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(partitionId))
        {
            _logger.LogPartitionIdRequired();
            return BadRequest("Partition id is required and cannot be empty.");
        }

        bool result = await _userPartitionService
            .InPartitionAsync(_userManager.NormalizeName(userName), partitionId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }
}