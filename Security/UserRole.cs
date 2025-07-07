using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using portal_agile.Models;
using Microsoft.AspNetCore.Identity;

namespace portal_agile.Security
{
    public class UserRole
    {
        public required string UserId { get; set; }

        public required string RoleId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
    }
}
