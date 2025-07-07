using Microsoft.AspNetCore.Mvc;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Roles;
using portal_agile.Dtos.UserPermissions.Requests;
using portal_agile.Dtos.Users;
using portal_agile.Dtos.Users.Requests;
using portal_agile.Exceptions.Permissions;
using portal_agile.Exceptions.Users;
using portal_agile.Services;

namespace portal_agile.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/all
        ///
        /// </remarks>
        /// <returns>All roles of a user</returns>
        /// <response code="200">Returns the requested users</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<UserCreateRequest>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var roles = await _userService.GetAllUsersAsync();
                return Ok(roles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a user by ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/{userId}
        ///
        /// </remarks>
        /// <returns>The user</returns>
        /// <param name="userId">The ID of the user</param>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{userId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCreateRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById([FromRoute] string userId)
        {
            try
            {
                var roles = await _userService.GetUserByIdAsync(userId);
                return Ok(roles);
            }
            catch (PermissionNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a user by email
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/by-email?email=john.doe@example.com
        ///
        /// </remarks>
        /// <returns>The user</returns>
        /// <param name="email">The email of the user</param>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("by-email")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCreateRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            try
            {
                var roles = await _userService.GetUserByEmailAsync(email);
                return Ok(roles);
            }
            catch (PermissionNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a user by phone number
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/by-phone-number?phoneNumber=+1234567890
        ///
        /// </remarks>
        /// <returns>The user</returns>
        /// <param name="phoneNumber">The phone number of the user</param>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("by-phone-number")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCreateRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByPhoneNumber([FromQuery] string phoneNumber)
        {
            try
            {
                var roles = await _userService.GetUserByPhoneNumberAsync(phoneNumber);
                return Ok(roles);
            }
            catch (PermissionNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a user by username
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/by-username?userName=john.doe
        ///
        /// </remarks>
        /// <returns>The user</returns>
        /// <param name="userName">The username of the user</param>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("by-username")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserCreateRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByUserName([FromQuery] string userName)
        {
            try
            {
                var roles = await _userService.GetUserByUserNameAsync(userName);
                return Ok(roles);
            }
            catch (PermissionNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves all roles of a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/{userId}/all-roles
        ///
        /// </remarks>
        /// <returns>All roles of a user</returns>
        /// <param name="userId">The ID of the user</param>
        /// <response code="200">Returns the requested roles</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{userId}/all-roles")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserRoles([FromRoute] string userId)
        {
            try
            {
                var roles = await _userService.GetRolesByUserIdAsync(userId);
                return Ok(roles);
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
        /// Retrieves all direct permissions of a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/{userId}/direct-permissions
        ///
        /// </remarks>
        /// <returns>All permissions of a user</returns>
        /// <param name="userId">The ID of the user</param>
        /// <response code="200">Returns the requested permissions</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{userId}/direct-permissions")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<UserWithPermissionsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserDirectPermissions([FromRoute] string userId)
        {
            try
            {
                var permissions = await _userService.GetUserDirectPermissionsAsync(userId);
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
        /// Retrieves all permissions of a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/{userId}/get-all-user-permissions/{userId}
        ///
        /// </remarks>
        /// <returns>All permissions of a role</returns>
        /// <param name="userId">The ID of the user</param>
        /// <response code="200">Returns the requested permissions</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Route("{userId}/all-permissions")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUserPermissions([FromBody] string userId)
        {
            try
            {
                var permissions = await _userService.GetAllUserPermissionsAsync(userId);
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
        /// Retrieves all permission claims of a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/permissions/get-user-permission-claims/{userId}
        ///
        /// </remarks>
        /// <returns>All permission claims of a user</returns>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <response code="200">Returns the requested permission claims</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Route("get-user-permission-claims")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserPermissionClaims([FromBody] string userId)
        {
            try
            {
                var claims = await _userService.GetUserPermissionClaimsAsync(userId);
                return Ok(claims);
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
        /// Create a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/store
        ///     {
        ///        "FirstName": "John",
        ///        "LastName": "Doe",
        ///        "UserName": "john.doe",
        ///        "Email": "john.doe@example.com",
        ///        "PhoneNumber": "+1234567890",
        ///     }
        ///
        /// </remarks>
        /// <returns>Newly created user</returns>
        /// <param name="request">The data of the user to be created</param>
        /// <response code="200">Returns the user data</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Route("store")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] UserCreateRequest request)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newUser = await _userService.Store(request);
                return Ok(newUser);
            }
            catch (InvalidUserCreateDataException)
            {
                return BadRequest("Invalid user data.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Update a user via key
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/{userId}/update-from-key
        ///     {
        ///        "key": "FirstName",
        ///        "newValue": "Johnny",
        ///     }
        ///
        /// </remarks>
        /// <param name="userId">The id of the user to revoke the permissions from</param>
        /// <param name="request">The request body</param>
        /// <response code="200">Returns the updated user when update is successful</response>
        /// <response code="400">If the user cannot be found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Route("{userId}/update-from-key")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserViaKey(
            [FromRoute] string userId,
            [FromBody] UserUpdateViaKeyRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(await _userService.UpdateFromKeyAsync(userId, request.Key, request.NewValue));
            }
            catch (InvalidUserIdException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Assign a role to a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/{userId}/assign-role
        ///     {
        ///        "roleName": "Admin",
        ///     }
        ///
        /// </remarks>
        /// <returns>True or false</returns>
        /// <param name="userId">The ID of the user</param>
        /// <param name="roleName">The name of the role to assign</param>
        /// <response code="200">Returns true when role has been assigned</response>
        /// <response code="404">If the user or role is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Route("{userId}/assign-role")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignRoleToUser([FromRoute] string userId, [FromBody] string roleName)
        {
            try
            {
                var assigned = await _userService.AssignRoleToUserAsync(userId, roleName);
                return Ok(assigned);
            }
            catch (PermissionNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Deactivate a user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/{userId}/deactivate
        ///
        /// </remarks>
        /// <param name="userId">The id of the user to deactivate</param>
        /// <response code="200">Returns true when user has been deactivated</response>
        /// <response code="400">If the user cannot be found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Route("{userId}/deactivate")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUser([FromRoute] string userId)
        {
            try
            {
                return Ok(await _userService.DeactivateUserAsync(userId));
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
