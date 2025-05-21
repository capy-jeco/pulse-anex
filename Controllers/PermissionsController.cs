using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Permissions;
using portal_agile.Security;

namespace portal_agile.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        private readonly IMapper _mapper;

        public PermissionsController(
            IPermissionService permissionService, 
            IMapper mapper)
        {
            _permissionService = permissionService;
            _mapper = mapper;
        }


        /// <summary>
        /// Retrieves all permissions.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/permissions/get-all
        ///
        /// </remarks>
        /// <returns>A list of permissions.</returns>
        /// <response code="200">Returns the list of permissions</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Route("all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPermissions()
        {
            return Ok(await _permissionService.GetAllPermissionsAsync());
        }

        /// <summary>
        /// Retrieves a specific permission by its ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/permissions/{permissionId}
        ///
        /// </remarks>
        /// <param name="permissionId">The ID of the permission to retrieve.</param>
        /// <returns>The permission with the specified ID.</returns>
        /// <response code="200">Returns the requested permission</response>
        /// <response code="404">If the permission is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Route("{permissionId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissionById(int permissionId)
        {
            try
            {
                if (permissionId <= 0)
                    return BadRequest("Invalid permission ID.");

                var permission = await _permissionService.GetPermissionByIdAsync(permissionId);
                var permissionDto = _mapper.Map<PermissionDto>(permission);

                return Ok(permissionDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all permissions grouped by module.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/permissions/get-all-by-module
        ///
        /// </remarks>
        /// <returns>All permissions grouped by module</returns>
        /// <response code="200">Returns the requested permissions</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Route("all-by-module")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Dictionary<string, List<PermissionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPermissionsByModule()
        {
            var permissions = await _permissionService.GetAllPermissionsByModuleAsync();
            return Ok(permissions);
        }

        /// <summary>
        /// Creates a new permission.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/permissions/create
        ///     {
        ///        "name": "Create User",
        ///        "code": "USERS.CREATE",
        ///        "module": "UserManagement",
        ///        "description": "Grants access to create a new user in the system",
        ///     }
        ///
        /// </remarks>
        /// <param name="permissionCreateDto">The permission data to create.</param>
        /// <returns>The created permission.</returns>
        /// <response code="201">Returns the newly created permission</response>
        /// <response code="400">If the permission data is invalid</response>
        [HttpPost]
        [Route("create")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Store([FromBody] PermissionCreateDto permissionCreateDto)
        {
            if (permissionCreateDto == null)
                return BadRequest("Permission data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var permission = await _permissionService.CreateAsync(permissionCreateDto);

            return CreatedAtAction(nameof(GetPermissionById), new { permissionId = permission.PermissionId }, permission);
        }

        /// <summary>
        /// Updates a permission.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/permissions/update
        ///     {
        ///        "name": "Create User",
        ///        "code": "USERS.CREATE",
        ///        "module": "UserManagement",
        ///        "description": "Grants access to create a new user in the system",
        ///     }
        ///
        /// </remarks>
        /// <param name="permissionUpdate">The permission data</param>
        /// <returns>The updated permission.</returns>
        /// <response code="201">Returns the newly created permission</response>
        /// <response code="400">If the permission data is invalid</response>
        [HttpPost]
        [Route("update")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] Permission permissionUpdate)
        {
            if (permissionUpdate == null)
                return BadRequest("Permission data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var permission = await _permissionService.UpdatePermissionAsync(permissionUpdate);
            return Ok(permission);
        }

        /// <summary>
        /// Assign permissions to role.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/permissions/assign-permissions-to-role
        ///     {
        ///        "roleId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
        ///        "permissionIds": [1, 2, 3],
        ///        "modifiedBy": "b1c2d3e4-f5g6-0987-4321-098765hijlkm",
        ///     }
        ///
        /// </remarks>
        /// <param name="roleId">The id of the role to assign the permissions to</param>
        /// <param name="permissionIds">The ids of the permissions to assign to the role</param>
        /// <param name="modifiedBy">The id of the user that performed the update</param>
        /// <returns>No content.</returns>
        /// <response code="201">No content.</response>
        /// <response code="400">If the permission data is invalid</response>
        [HttpPost]
        [Route("assign-permissions-to-role")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignPermissionsToRoleAsync(string roleId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            if (string.IsNullOrEmpty(roleId))
                return BadRequest("Role ID is required.");

            if (permissionIds == null || !permissionIds.Any())
                return BadRequest("Permission IDs are required.");

            var success = await _permissionService.AssignPermissionsToRoleAsync(roleId, permissionIds, modifiedBy);
            if (!success)
                return BadRequest("Failed to assign permissions to role.");

            return NoContent(); // or Ok(true)
        }

        /// <summary>
        /// Check if user have the permission.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/permissions/has-permission
        ///     {
        ///        "userId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
        ///        "permissionCode": "USERS.CREATE",
        ///     }
        ///
        /// </remarks>
        /// <param name="userId">The id of the user</param>
        /// <param name="permissionCode">The permission code to check</param>
        /// <returns>No content.</returns>
        /// <response code="201">True/False</response>
        /// <response code="400">If the permission data is invalid</response>
        /// <response code="500">If the request had error</response>
        [HttpPost]
        [Route("has-permission")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HasPermission([FromBody] string userId, string permissionCode)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required.");
            if (string.IsNullOrEmpty(permissionCode))
                return BadRequest("Permission code is required.");
            var hasPermission = await _permissionService.HasPermissionAsync(userId, permissionCode);
            return Ok(hasPermission);
        }
    }
}
