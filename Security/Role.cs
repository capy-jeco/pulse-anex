using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

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

        [Display(Name = "Description")]
        [StringLength(200)]
        public required string Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is System Role")]
        public bool IsSystemRole { get; set; } = false;

        [Display(Name = "IsDeleted")]
        public bool IsDeleted { get; set; } = false;

        // Navigation collection for role permissions
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
