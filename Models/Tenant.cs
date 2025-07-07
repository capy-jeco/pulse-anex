using portal_agile.Security;
using System.ComponentModel.DataAnnotations;

namespace portal_agile.Models
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Subdomain { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ConnectionString { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsSystem { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<User> Users { get; set; } = new List<User>();

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
