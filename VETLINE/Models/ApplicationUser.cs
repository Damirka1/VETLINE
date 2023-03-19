using Microsoft.AspNetCore.Identity;

namespace VETLINE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
