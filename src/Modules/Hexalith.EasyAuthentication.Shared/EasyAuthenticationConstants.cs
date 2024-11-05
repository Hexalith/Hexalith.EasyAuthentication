namespace Hexalith.EasyAuthentication.Shared;

/// <summary>
/// Contains constants used in Easy Authentication.
/// </summary>
public static class EasyAuthenticationConstants
{
    /// <summary>
    /// The header name for the client principal ID.
    /// </summary>
    public const string ClientPrincipalHeader = "X-MS-CLIENT-PRINCIPAL";

    /// <summary>
    /// The header name for the client principal identity provider.
    /// </summary>
    public const string ClientPrincipalIdentityProviderHeader = "X-MS-CLIENT-PRINCIPAL-IDP";

    /// <summary>
    /// The header name for the client principal ID.
    /// </summary>
    public const string ClientPrincipalIdHeader = "X-MS-CLIENT-PRINCIPAL-ID";

    /// <summary>
    /// The header name for the client principal name.
    /// </summary>
    public const string ClientPrincipalNameHeader = "X-MS-CLIENT-PRINCIPAL-NAME";

    /// <summary>
    /// The header name for the partition ID.
    /// </summary>
    public const string PartitionHeader = "HEXALITH-PARTITION-ID";
}