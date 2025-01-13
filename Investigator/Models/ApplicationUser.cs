using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string DisplayName { get; set; }
        public bool IsBlocked { get; set; } = false;
        public DateTime LastSeen { get; set; }
        public string? SalesForceUserId { get; set; }    
        [NotMapped]
        public string Interval { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
