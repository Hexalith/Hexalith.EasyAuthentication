// <copyright file="HexalithApplicationTest.cs" company="Hexalith">
//     Copyright (c) Hexalith. All rights reserved.
//     Licensed under the MIT license.
//     See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.Security.UnitTests.Modules;

using FluentAssertions;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.Application.Configurations;
using Hexalith.Security.WebApp;
using Hexalith.Security.WebServer;
using Hexalith.UI.Components;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class HexalithApplicationTest
{
    [Fact]
    public void HexalithApplicationShouldReturnClientModuleTypes()
    {
        _ = HexalithApplication.WebAppApplication.WebAppModules
            .Should()
            .HaveCount(2);
        _ = HexalithApplication.WebAppApplication.Modules
            .Should()
            .HaveCount(2);
        _ = HexalithApplication.WebAppApplication.WebAppModules
            .Should()
            .Contain(typeof(HexalithSecurityWebAppModule));
        _ = HexalithApplication.WebAppApplication.Modules
            .Should()
            .Contain(typeof(HexalithSecurityWebAppModule));
    }

    [Fact]
    public void HexalithApplicationShouldReturnServerModuleTypes()
    {
        _ = HexalithApplication.WebServerApplication.WebServerModules
            .Should()
            .HaveCount(2);
        _ = HexalithApplication.WebServerApplication.Modules
            .Should()
            .HaveCount(2);
        _ = HexalithApplication.WebServerApplication.WebServerModules
            .Should()
            .Contain(typeof(HexalithSecurityWebServerModule));
        _ = HexalithApplication.WebServerApplication.Modules
            .Should()
            .Contain(typeof(HexalithSecurityWebServerModule));
    }

    [Fact]
    public void WebAppServicesFromModulesShouldBeAdded()
    {
        ServiceCollection services = [];
        Dictionary<string, string> inMemorySettings = new()
        {
            { $"{SecuritySettings.ConfigurationName()}:Enabled", "true" },
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        HexalithApplication.AddWebAppServices(services, configuration);

        // Check that the client module services have been added by checking if Fluent UI ToastService has been added
        _ = services
            .Should()
            .ContainSingle(s => s.ServiceType == typeof(MenuItemInformation));
    }
}