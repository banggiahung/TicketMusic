using Microsoft.AspNetCore.Identity;

namespace TicketMusic.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatartPath { get; set; }
        public string? FullName { get; set; }
        public string? Address { set; get;}
        public bool IsActive { get; set; }
        public string? RoleName { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
