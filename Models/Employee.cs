using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace portal_agile.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        // Foreign key relationship with User
        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Employee Number")]
        public string? EmployeeNumber { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [Display(Name = "Position")]
        [StringLength(100)]
        public required string Position { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Salary")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Display(Name = "Manager")]
        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public virtual Employee? Manager { get; set; }

        // Navigation collection for direct reports
        public virtual ICollection<Employee>? DirectReports { get; set; }
    }
}
