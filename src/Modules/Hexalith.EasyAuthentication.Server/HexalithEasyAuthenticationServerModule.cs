namespace Hexalith.EasyAuthentication.Server;

using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;
using Hexalith.EasyAuthentication.Server.Middlewares;
using Hexalith.EasyAuthentication.Shared.Configurations;
using Hexalith.Extensions.Helpers;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

using NEasyAuthMiddleware;

/// <summary>
/// Microsoft Easy Authentication server module.
/// </summary>
public sealed class HexalithEasyAuthenticationServerModule : IServerApplicationModule
{
    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => "Microsoft Easy Authentication server module";

    /// <inheritdoc/>
    public string Id => "Hexalith.EasyAuthentication.Server";

    /// <inheritdoc/>
    public string Name => "Microsoft Easy Authentication server";

    /// <inheritdoc/>
    public int OrderWeight => 0;

    /// <inheritdoc/>
    string IApplicationModule.Path => HexalithEasyAuthenticationServerModule.Path;

    /// <inheritdoc/>
    public IEnumerable<Assembly> PresentationAssemblies => [GetType().Assembly];

    /// <inheritdoc/>
    public string Version => "1.0";

    private static string CookieScheme => "Cookies";

    private static string EasyAuthenticationScheme => "MicrosoftEasyAuthentication";

    private static string Path => "Hexalith/EasyAuthentication";

    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        EasyAuthenticationSettings settings = configuration.GetSettings<EasyAuthenticationSettings>()
            ?? throw new InvalidOperationException($"Could not load settings section '{EasyAuthenticationSettings.ConfigurationName()}'");
        if (!settings.Enabled)
        {
            return;
        }

        if (settings.UseMsal)
        {
            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();
        }
        else
        {
            _ = services
                .AddEasyAuth()
                .AddIdentityCookies();
        }

        _ = services.AddAuthorization();
        _ = services.AddScoped<AuthenticationStateProvider, ServerPersistingAuthenticationStateProvider>();
    }

    /// <inheritdoc/>
    public void UseModule(object builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (builder is not ApplicationBuilder b)
        {
            throw new InvalidOperationException($"Invalid builder type '{builder.GetType().FullName}'. The expected type is ${nameof(ApplicationBuilder)}.");
        }

        IOptions<EasyAuthenticationSettings> settings = b.ApplicationServices.GetRequiredService<IOptions<EasyAuthenticationSettings>>();
        if (settings.Value.UseMsal)
        {
            IWebHostEnvironment env = b.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                _ = b.UseMiddleware<DevelopmentAuthenticationMiddleware>();
            }

            _ = b.UseMiddleware<ContainerAppsAuthenticationMiddleware>();
        }
    }
}