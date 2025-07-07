using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portal_agile.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Label { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Route { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        public int? ParentId { get; set; }

        public int? ModuleId { get; set; }

        public int MenuLevel { get; set; } = 0; // Root is level 0

        public int SortOrder { get; set; } = 0; // Position within siblings

        // Permission Key/Slug (e.g., "user.create", "payroll.generate")
        [StringLength(100)]
        public string? RequiredPermission { get; set; }

        [StringLength(255)]
        public string? Tooltip { get; set; }

        public bool IsVisible { get; set; } = true;

        public bool IsActive { get; set; } = true;

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ModuleId")]
        public Module? Module { get; set; }

        [ForeignKey("ParentId")]
        [InverseProperty("Children")]
        public MenuItem? Parent { get; set; }

        [InverseProperty("Parent")]
        public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

        public ICollection<MenuItemPermission> MenuItemPermissions { get; set; } = new List<MenuItemPermission>();
    }
}
