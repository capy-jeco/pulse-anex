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
        public async Task<IActionResult> CreateAsync([FromBody] PermissionCreateDto permissionCreateDto)
        {
            if (permissionCreateDto == null)
                return BadRequest("Permission data is required.");

            var permission = _mapper.Map<Permission>(permissionCreateDto);

            await _permissionService.Store(permission);

            return CreatedAtAction(nameof(GetPermissionById), new { permissionId = permission.PermissionId }, permission);
        }

        [HttpGet]
        [Route("get-by-id/{permissionId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Permission), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissionById(int permissionId)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(permissionId);

            var permissionDto = _mapper.Map<PermissionDto>(permission);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }
    }
}
