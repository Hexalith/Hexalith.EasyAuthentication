namespace Hexalith.Security.WebServer.Controllers;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Hexalith.Application.Sessions.Services;

using Microsoft.AspNetCore.Http;
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
    private readonly IUserPartitionService _userPartitionService;

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
        ILogger<UserPartitionController> logger)
    {
        ArgumentNullException.ThrowIfNull(userPartitionService);
        _userPartitionService = userPartitionService;
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

        return Ok(await _userPartitionService
            .GetDefaultPartitionAsync(userName, cancellationToken)
            .ConfigureAwait(false));
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
            return BadRequest("User name is required and cannot be empty.");
        }

        return Ok(await _userPartitionService
            .GetPartitionsAsync(userName, cancellationToken)
            .ConfigureAwait(false));
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
            return BadRequest("User name is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(partitionId))
        {
            _logger.LogPartitionIdRequired();
            return BadRequest("Partition id is required and cannot be empty.");
        }

        return Ok(await _userPartitionService
            .InPartitionAsync(userName, partitionId, cancellationToken)
            .ConfigureAwait(false));
    }
}