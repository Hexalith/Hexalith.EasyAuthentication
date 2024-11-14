namespace Hexalith.Security.SharedAssets.Configurations;

using System.Runtime.Serialization;

using Hexalith.Extensions.Configuration;

/// <summary>
/// Easy Authentication settings.
/// </summary>
[DataContract]
public record SecuritySettings(
    [property: DataMember]
    bool Enabled) : ISettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecuritySettings"/> class.
    /// </summary>
    public SecuritySettings()
        : this(false)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use Microsoft Authentication Library (MSAL).
    /// </summary>
    public bool UseMsal { get; set; }

    /// <summary>
    /// The name of the configuration.
    /// </summary>
    /// <returns>Settings section name.</returns>
    public static string ConfigurationName() => nameof(Hexalith) + ":" + nameof(Security);
}