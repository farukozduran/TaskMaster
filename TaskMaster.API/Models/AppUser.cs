using Microsoft.AspNetCore.Identity;

namespace TaskMaster.API.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }

        public int? DeveloperId { get; set; }
        public Developer Developer { get; set; }
    }
}
