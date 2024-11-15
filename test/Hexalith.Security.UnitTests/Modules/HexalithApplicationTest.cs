// <copyright file="HexalithApplicationTest.cs" company="Hexalith">
//     Copyright (c) Hexalith. All rights reserved.
//     Licensed under the MIT license.
//     See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.Security.UnitTests.Modules;

using FluentAssertions;

using Hexalith.Application.Modules.Applications;
using Hexalith.Security.SharedUIElements.Modules;
using Hexalith.Security.WebApp;
using Hexalith.Security.WebServer;
using Hexalith.UI.Components.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

public class HexalithApplicationTest
{
    [Fact]
    public void HexalithApplicationShouldReturnClientModuleTypes()
    {
        _ = HexalithApplication.WebAppApplication.WebAppModules
            .Should()
            .HaveCount(1);
        _ = HexalithApplication.WebAppApplication.Modules
            .Should()
            .HaveCount(3);
        _ = HexalithApplication.WebAppApplication.WebAppModules
            .Should()
            .Contain(typeof(HexalithSecurityWebAppModule));
        _ = HexalithApplication.WebAppApplication.Modules
            .Should()
            .Contain(typeof(HexalithSecuritySharedModule));
        _ = HexalithApplication.WebAppApplication.Modules
            .Should()
            .Contain(typeof(HexalithSecurityWebAppModule));
        _ = HexalithApplication.WebAppApplication.Modules
            .Should()
            .Contain(typeof(HexalithUIComponentsSharedModule));
    }

    [Fact]
    public void HexalithApplicationShouldReturnServerModuleTypes()
    {
        _ = HexalithApplication.WebServerApplication.WebServerModules
            .Should()
            .HaveCount(1);
        _ = HexalithApplication.WebServerApplication.Modules
            .Should()
            .HaveCount(3);
        _ = HexalithApplication.WebServerApplication.WebServerModules
            .Should()
            .Contain(typeof(HexalithSecurityServerModule));
        _ = HexalithApplication.WebServerApplication.Modules
            .Should()
            .Contain(typeof(HexalithSecuritySharedModule));
        _ = HexalithApplication.WebServerApplication.Modules
            .Should()
            .Contain(typeof(HexalithUIComponentsSharedModule));
    }

    [Fact]
    public void WebAppServicesFromModulesShouldBeAdded()
    {
        ServiceCollection services = [];
        Mock<IConfiguration> configurationMock = new(MockBehavior.Strict);

        // Mock the configuration GetSection method
        _ = configurationMock
            .Setup(c => c.GetSection(It.IsAny<string>()))
            .Returns(new Mock<IConfigurationSection>().Object);

        HexalithApplication.AddWebAppServices(services, configurationMock.Object);

        // Check that the client module services have been added by checking if AuthenticationStateProvider has been added
        _ = services
            .Should()
            .ContainSingle(s => s.ServiceType == typeof(WebAppPersistentAuthenticationStateProvider));
    }
}