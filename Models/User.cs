using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using portal_agile.Security;

namespace portal_agile.Models
{
    public class User : IdentityUser
    {
        [Required]
        public int TenantId { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        public required string FirstName { get; set; }

        public string? MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        public required string LastName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation property for Tenant
        public virtual Tenant Tenant { get; set; } = null!;

        // Navigation property to Employee if this user is an employee
        public virtual Employee Employee { get; set; } = null!;

        // Navigation collection for user roles
        public virtual ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<Role> Roles => UserRoles.Select(ur => ur.Role).ToList();
    }
}
