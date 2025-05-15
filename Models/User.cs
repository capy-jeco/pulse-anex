using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using portal_agile.Security;

namespace portal_agile.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "First Name")]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation property to Employee if this user is an employee
        public virtual Employee? Employee { get; set; }

        // Navigation collection for user permissions (direct permissions not through roles)
        public virtual ICollection<UserPermission>? DirectPermissions { get; set; }
    }
}
