namespace Hexalith.EasyAuthentication.Server.Models;

using System.Runtime.Serialization;

/// <summary>
/// Represents a claim associated with the client principal.
/// </summary>
[DataContract]
public record ClientPrincipalClaim(
    [property: DataMember(Name = "typ")] string Type,
    [property: DataMember(Name = "val")] string Value)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientPrincipalClaim"/> class.
    /// </summary>
    public ClientPrincipalClaim()
        : this(string.Empty, string.Empty)
    {
    }
}