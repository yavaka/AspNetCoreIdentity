using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class User : IdentityUser
    {
        public string Locale { get; set; } = "en-GB";

        public string OrganizationId { get; set; }

    }
}