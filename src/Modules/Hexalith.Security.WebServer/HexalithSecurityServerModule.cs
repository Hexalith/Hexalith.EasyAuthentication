namespace Hexalith.Security.WebServer;

using System.Collections.Generic;
using System.Reflection;

using Hexalith.Application.Modules.Modules;
using Hexalith.Extensions.Configuration;
using Hexalith.Extensions.Helpers;
using Hexalith.Security.Application.Configurations;
using Hexalith.Security.Server.Middlewares;
using Hexalith.Security.UI.Components.Claims;
using Hexalith.Security.UI.Components.Menu;
using Hexalith.Security.UI.Pages.Security;
using Hexalith.Security.WebServer.Middlewares;

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
/// Microsoft Security server module.
/// </summary>
public sealed class HexalithSecurityServerModule : IWebServerApplicationModule
{
    /// <inheritdoc/>
    public IEnumerable<string> Dependencies => [];

    /// <inheritdoc/>
    public string Description => "Microsoft Security server module";

    /// <inheritdoc/>
    public string Id => "Hexalith.Security.Server";

    /// <inheritdoc/>
    public string Name => "Microsoft Security server";

    /// <inheritdoc/>
    public int OrderWeight => 0;

    /// <inheritdoc/>
    string IApplicationModule.Path => Path;

    /// <inheritdoc/>
    public IEnumerable<Assembly> PresentationAssemblies => [GetType().Assembly, typeof(SecurityIndex).Assembly, typeof(ClaimsView).Assembly];

    /// <inheritdoc/>
    public string Version => "1.0";

    private static string Path => "Hexalith/Security";

    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        SecuritySettings settings = configuration.GetSettings<SecuritySettings>()
            ?? throw new InvalidOperationException($"Could not load settings section '{SecuritySettings.ConfigurationName()}'");
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
        _ = services.AddScoped<AuthenticationStateProvider, ServerPersistingAuthenticationStateProvider>()
            .AddSingleton(p => SecurityMenu.Menu)
            .ConfigureSettings<SecuritySettings>(configuration);
    }

    /// <inheritdoc/>
    public void UseModule(object builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (builder is not WebApplication app)
        {
            throw new InvalidOperationException($"Invalid builder type '{builder.GetType().FullName}'. The expected type is {typeof(WebApplication).FullName}.");
        }

        // initialize modules
        using IServiceScope scope = app.Services.CreateScope();

        IOptions<SecuritySettings> settings = scope.ServiceProvider.GetRequiredService<IOptions<SecuritySettings>>();
        if (settings.Value.UseMsal)
        {
            IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                _ = app.UseMiddleware<DevelopmentAuthenticationMiddleware>();
            }

            _ = app.UseMiddleware<ContainerAppsAuthenticationMiddleware>();
        }
    }
}