using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace portal_agile.Security
{
    public class RolePermission
    {
        [Key]
        public int RolePermissionId { get; set; }

        [Required]
        public required string RoleId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        [Display(Name = "Created By")]
        public required string CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission? Permission { get; set; }
    }
}
