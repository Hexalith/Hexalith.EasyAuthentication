namespace Hexalith.Security.UI.Components.Partitions;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Service for managing users.
/// </summary>
public class PartitionService : IPartitionService
{
    /// <summary>
    /// Gets all users asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of user summary view models.</returns>
    public Task<IEnumerable<PartitionSummaryViewModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<PartitionSummaryViewModel>>([
            new PartitionSummaryViewModel("1", "Partition 1"),
            new PartitionSummaryViewModel("2", "Partition 2"),
            new PartitionSummaryViewModel("3", "Partition 3")
        ]);
    }
}