using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace portal_agile.Dtos.Roles
{

    [MetadataType(typeof(RoleCreateDtoMetadata))]
    public class RoleupdateDto : RoleDto
    {
        // Constructor to initialize required properties
        public RoleupdateDto()
        {
            // Initialize with default values
            Name = string.Empty;
            Description = string.Empty;
            IsSystemRole = false;
        }
    }

    // Separate class for metadata/validation attributes
    public class RoleupdateDtoMetadata
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        [Display(Name = "Role Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        [Display(Name = "Description")]
        public required string Description { get; set; }
    }
}
