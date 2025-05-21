using System.ComponentModel.DataAnnotations;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Dtos.UserPermissions.Requests
{
    public class AssignPermissionToUserRequest
    {
        [Required(ErrorMessage = "PermissionId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PermissionId must be a positive number.")]
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "ModifiedBy is required.")]
        public required string ModifiedBy { get; set; }
    }
}
