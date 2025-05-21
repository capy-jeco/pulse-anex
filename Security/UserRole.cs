using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using portal_agile.Models;
using Microsoft.AspNetCore.Identity;

namespace portal_agile.Security
{
    public class UserRole
    {
        [Key]
        public required string UserRoleId { get; set; }

        public required string UserId { get; set; } = default!;

        public required string RoleId { get; set; } = default!;

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

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
    }
}
