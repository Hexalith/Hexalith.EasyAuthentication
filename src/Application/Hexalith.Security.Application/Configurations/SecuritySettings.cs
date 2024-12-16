namespace Hexalith.Security.Application.Configurations;

using System.Runtime.Serialization;

using Hexalith.Extensions.Configuration;

/// <summary>
/// Security settings.
/// </summary>
[DataContract]
public record SecuritySettings(
    [property: DataMember]
    bool Disabled) : ISettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecuritySettings"/> class.
    /// </summary>
    public SecuritySettings()
        : this(false)
    {
    }

    /// <summary>
    /// The name of the configuration.
    /// </summary>
    /// <returns>Settings section name.</returns>
    public static string ConfigurationName() => nameof(Hexalith) + ":" + nameof(Security);
}