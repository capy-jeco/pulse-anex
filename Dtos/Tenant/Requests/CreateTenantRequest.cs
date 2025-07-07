using System.ComponentModel.DataAnnotations;

namespace portal_agile.Dtos.Tenant.Requests
{
    public class CreateTenantRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "Subdomain can only contain letters, numbers, and hyphens")]
        public string Subdomain { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ConnectionString { get; set; }

        // Admin user for the tenant
        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AdminFirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AdminLastName { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string AdminPassword { get; set; } = string.Empty;
    }
}
