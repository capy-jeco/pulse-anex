using Microsoft.AspNetCore.Mvc;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Roles;
using portal_agile.Security;

namespace portal_agile.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Retrieves all roles.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/roles/all
        ///
        /// </remarks>
        /// <returns>All roles in the system</returns>
        /// <response code="200">Returns the requested roles</response>
        /// <response code="404">If no role found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieve a role via its id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/roles/{roleId}
        ///
        /// </remarks>
        /// <returns>A role.</returns>
        /// <response code="200">Returns the requested role</response>
        /// <response code="404">If the role is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{roleId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoleById([FromRoute] string roleId)
        {
            try
            {
                var roles = await _roleService.GetRoleByIdAsync(roleId);
                return Ok(roles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves all permissions of a role.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/roles/{roleId}/role-permissions
        ///
        /// </remarks>
        /// <returns>All permissions of a role</returns>
        /// <param name="roleId">The ID of the role.</param>
        /// <response code="200">Returns the requested permissions</response>
        /// <response code="404">If the role is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Route("{roleId}/role-permissions")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRolePermissions([FromRoute] string roleId)
        {
            try
            {
                var permissions = await _roleService.GetRolePermissionsAsync(roleId);
                return Ok(permissions);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Creates a role.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/roles/store
        ///
        /// </remarks>
        /// <returns>Created role</returns>
        /// <param name="roleCreateDto"> The role to create.</param>
        /// <response code="200">Returns the created role</response>
        /// <response code="404">If the role is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Route("create")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Store([FromBody] RoleCreateDto roleCreateDto)
        {
            try
            {
                var createdRole = await _roleService.CreateRoleAsync(roleCreateDto);
                return CreatedAtAction(nameof(GetRoleById), new { roleId = createdRole.Id }, createdRole);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Update a role.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/roles/{roleId}/update
        ///
        /// </remarks>
        /// <returns>Created role</returns>
        /// <param name="roleId"> The role id to update.</param>
        /// <param name="roleUpdate"> The role object for update.</param>
        /// <response code="200">Returns the created role</response>
        /// <response code="404">If the role is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Route("{roleId}/update")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] string roleId, [FromBody] RoleDto roleUpdate)
        {
            try
            {
                var updatedRole = await _roleService.UpdateRoleAsync(roleId, roleUpdate);
                return Ok(updatedRole);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }


    }
}
