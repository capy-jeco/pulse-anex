using Microsoft.AspNetCore.Identity;
using portal_agile.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portal_agile.Security
{
    public class Role : IdentityRole
    {
        public Role() : base()
        {
            RolePermissions = new HashSet<RolePermission>();
        }

        public Role(string roleName) : base(roleName)
        {
            RolePermissions = new HashSet<RolePermission>();
        }

        public required int TenantId { get; set; }

        [Display(Name = "Description")]
        [StringLength(200)]
        public required string Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is System Role")]
        public bool IsSystemRole { get; set; } = false;

        [Display(Name = "IsDeleted")]
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; } = null!;

        public virtual ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<User> Users => UserRoles.Select(ur => ur.User).ToList();

        // Navigation collection for role permissions
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
