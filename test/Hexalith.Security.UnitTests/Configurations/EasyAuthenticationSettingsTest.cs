namespace Hexalith.Security.UnitTests.Configurations;

using System.Text.Json;

using FluentAssertions;

using Hexalith.Security.SharedUIElements.Configurations;
using Hexalith.Extensions.Helpers;
using Hexalith.TestMocks;

using Microsoft.Extensions.Configuration;

public class SecuritySettingsTest : SerializationTestBase
{
    public static Dictionary<string, string> TestSettings => new()
        {
            { "Hexalith:Security:Enabled", "true" },
        };

    [Fact]
    public void GetSettingsFromConfigurationShouldSucceed()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(SecuritySettingsTest.TestSettings)
            .Build();
        SecuritySettings settings = configuration.GetSettings<SecuritySettings>();
        _ = settings.Should().NotBeNull();
        _ = settings.Enabled.Should().BeTrue();
    }

    [Fact]
    public void ShouldDeserialize()
    {
        // Arrange
        string json = "{ \"Enabled\": true}";

        // Act
        SecuritySettings settings = JsonSerializer.Deserialize<SecuritySettings>(json);

        // Assert
        _ = settings.Should().NotBeNull();
        _ = settings.Enabled.Should().BeTrue();
    }

    public override object ToSerializeObject() => new SecuritySettings
    {
        Enabled = true,
    };
}