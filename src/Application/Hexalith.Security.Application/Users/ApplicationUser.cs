using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string ContactId { get; set; }
}