using System.ComponentModel.DataAnnotations;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Dtos.UserPermissions.Requests
{
    public class RevokePermissionFromUserRequest
    {
        [Required(ErrorMessage = "Permission IDs are required.")]
        [MinLength(1, ErrorMessage = "At least one permission ID must be provided.")]
        public required IEnumerable<int> PermissionIds { get; set; }

        [Required(ErrorMessage = "ModifiedBy is required.")]
        public required string ModifiedBy { get; set; }
    }
}
