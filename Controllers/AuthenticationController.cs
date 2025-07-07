using Microsoft.AspNetCore.Mvc;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Authentication.Requests;
using portal_agile.Dtos.Authentication.Response;
using portal_agile.Dtos.Users;
using portal_agile.Dtos.Users.Requests;
using portal_agile.Exceptions.Authentication;
using portal_agile.Exceptions.Users;
using portal_agile.Services;
using System.Security.Authentication;

namespace portal_agile.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(
            ILogger<AuthenticationController> logger, 
            IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticate a user and return a JWT token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/authentication/login
        ///     {
        ///        "Email": "john.doe@example.com",
        ///        "Password": "P4s$w0rD",
        ///     }
        ///
        /// </remarks>
        /// <returns>Login Successful Response</returns>
        /// <param name="request">The data of the user to be created</param>
        /// <response code="200">If the login is successful</response>
        /// <response code="401">If credentials are invalid</response>
        /// <response code="400">If request data is invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Route("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var loginResponse = await _authenticationService.LoginAsync(request);
                return Ok(loginResponse);
            }
            catch (InvalidCredentialException)
            {
                // Generic message to prevent user enumeration
                return Unauthorized(new { message = "Invalid email or password." });
            }
            catch (UserLockedException ex)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    message = "Account is temporarily locked due to too many failed attempts.",
                    lockoutEnd = ex.LockoutEnd?.ToString("yyyy-MM-ddTHH:mm:ssZ")
                });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
