using portal_agile.Security;
using System.ComponentModel.DataAnnotations;

namespace portal_agile.Models
{
    public class Module
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string ModuleName { get; set; }

        [Required]
        [StringLength(100)]
        public required string DisplayName { get; set; }

        [Required]
        [StringLength(500)]
        public required string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}
