namespace Hexalith.EasyAuthentication.WebServer.Models;

using System.Runtime.Serialization;

/// <summary>
/// Represents the client principal information.
/// </summary>
[DataContract]
public record ClientPrincipal(
    [property: DataMember(Name = "auth_typ")] string IdentityProvider,
    [property: DataMember(Name = "claims")] IEnumerable<ClientPrincipalClaim> Claims,
    [property: DataMember(Name = "name_typ")] string NameClaimType,
    [property: DataMember(Name = "role_typ")] string RoleClaimType)
{
}