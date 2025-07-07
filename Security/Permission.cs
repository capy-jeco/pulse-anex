using portal_agile.Models;
using System.ComponentModel.DataAnnotations;

namespace portal_agile.Security
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; } = 0;

        [Required]
        [StringLength(100)]
        [Display(Name = "Permission Name")]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Permission Code")]
        public required string Code { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation collections

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];

        public virtual ICollection<MenuItemPermission> MenuItemPermissions { get; set; } = [];
    }
}
