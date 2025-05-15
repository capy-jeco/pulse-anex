using System.ComponentModel.DataAnnotations;

namespace portal_agile.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Department Name")]
        public required string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public required string Description { get; set; }

        // Navigation collection for employees in this department
        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
