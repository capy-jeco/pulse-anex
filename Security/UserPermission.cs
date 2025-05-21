using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using portal_agile.Models;

namespace portal_agile.Security
{
    public class UserPermission
    {
        [Key]
        public int UserPermissionId { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        [Display(Name = "Created By")]
        public required string CreatedBy { get; set; }

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; } = null!;
    }
}
