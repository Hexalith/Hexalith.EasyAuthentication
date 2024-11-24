namespace Hexalith.Security.ApiServer;

using System.Collections.Generic;

using Dapr.Actors.Runtime;

using Hexalith.Application.Modules.Modules;
using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.Extensions.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Partitions.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Sessions.Helpers;
using Hexalith.Security.Application;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Microsoft Security server module.
/// </summary>
public sealed class HexalithSecurityApiServerModule : IApiServerApplicationModule
{
    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => "Microsoft Security API Server module";

    /// <inheritdoc/>
    public string Id => $"{HexalithSecurityApplicationInformation.Id}.ApiServer";

    /// <inheritdoc/>
    public string Name => $"{HexalithSecurityApplicationInformation.Name} Api Server";

    /// <inheritdoc/>
    public int OrderWeight => 0;

    /// <inheritdoc/>
    string IApplicationModule.Path => Path;

    /// <inheritdoc/>
    public string Version => field ??= this.ProductVersion() ?? "1.0";

    private static string Path => HexalithSecurityApplicationInformation.ShortName;

    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        _ = services.AddDaprIdentityStore();
    }

    /// <summary>
    /// Registers the actors associated with the module.
    /// </summary>
    /// <param name="actorCollection">The actor collection.</param>
    public static void RegisterActors(object actorCollection)
    {
        ArgumentNullException.ThrowIfNull(actorCollection);
        if (actorCollection is not ActorRegistrationCollection actorRegistrations)
        {
            throw new ArgumentException($"{nameof(RegisterActors)} parameter must be an {nameof(ActorRegistrationCollection)}. Actual type : {actorCollection.GetType().Name}.", nameof(actorCollection));
        }

        actorRegistrations.RegisterSessionActors();
        actorRegistrations.RegisterPartitionActors();
        actorRegistrations.RegisterIdentityActors();
    }

    /// <inheritdoc/>
    public void UseModule(object application)
    {
    }
}