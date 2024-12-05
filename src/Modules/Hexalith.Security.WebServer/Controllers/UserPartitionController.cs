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
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when userPartitionService or logger is null.</exception>
    public UserPartitionController(IUserPartitionService userPartitionService, ILogger<UserPartitionController> logger)
    {
        _userPartitionService = userPartitionService ?? throw new ArgumentNullException(nameof(userPartitionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the default partition for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The default partition.</returns>
    [HttpGet("{userId}/default")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> GetDefaultPartitionAsync(
        [Required][FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogUserIdRequired();
            return BadRequest("User id is required and cannot be empty.");
        }

        string? result = await _userPartitionService
            .GetDefaultPartitionAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Gets all partitions for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of partitions.</returns>
    [HttpGet("{userId}/partitions")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<string>>> GetPartitionsAsync(
        [Required][FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogUserIdRequired();
            return BadRequest("User id is required and cannot be empty.");
        }

        IEnumerable<string> result = await _userPartitionService
            .GetPartitionsAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    /// Checks if a user is in a specific partition.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="partitionId">The partition identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the user is in the partition, otherwise false.</returns>
    [HttpGet("{userId}/in-partition/{partitionId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> InPartitionAsync(
        [Required][FromRoute] string userId,
        [Required][FromRoute] string partitionId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
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
            .InPartitionAsync(userId, partitionId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }
}