using System.ComponentModel.DataAnnotations;

namespace portal_agile.Dtos.Permissions
{
    [MetadataType(typeof(PermissionCreateDtoMetadata))]
    public class PermissionCreateDto : PermissionDto
    {
        // Constructor to initialize required properties
        public PermissionCreateDto()
        {
            // Initialize with default values
            Name = string.Empty;
            Code = string.Empty;
            ModuleName = string.Empty;
            Description = string.Empty;
        }
    }

    // Separate class for metadata/validation attributes
    public class PermissionCreateDtoMetadata
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        [Display(Name = "Permission Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        [StringLength(100)]
        [Display(Name = "Permission Code")]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Module is required.")]
        [StringLength(100)]
        [Display(Name = "Module")]
        public required string Module { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        [Display(Name = "Description")]
        public required string Description { get; set; }
    }
}
