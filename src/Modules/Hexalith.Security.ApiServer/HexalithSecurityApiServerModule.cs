namespace Hexalith.Security.ApiServer;

using System.Collections.Generic;

using Dapr.Actors.Runtime;

using Hexalith.Application.Modules.Modules;
using Hexalith.DaprIdentityStore.Helpers;
using Hexalith.DaprIdentityStore.Models;
using Hexalith.Extensions.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Partitions.Helpers;
using Hexalith.Infrastructure.DaprRuntime.Sessions.Helpers;
using Hexalith.Security.Application;
using Hexalith.Security.Servers.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Microsoft Security server module.
/// </summary>
public sealed class HexalithSecurityApiServerModule : IApiServerApplicationModule
{
    private static string? _version;

    /// <inheritdoc/>
    public IDictionary<string, AuthorizationPolicy> AuthorizationPolicies => new Dictionary<string, AuthorizationPolicy>();

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
    public string Version => _version ??= GetType().Assembly.GetAssemblyVersion() ?? "1.0.0";

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
        _ = services.AddIdentityApiEndpoints<CustomUser>()
            .AddUserStore<DaprIdentityStore.Stores.DaprActorUserStore>();
        _ = services.AddDaprIdentityStore();
        _ = services.AddControllers().AddApplicationPart(typeof(UserPartitionController).Assembly);
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