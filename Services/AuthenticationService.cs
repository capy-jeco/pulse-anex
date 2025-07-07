using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Authentication.Requests;
using portal_agile.Dtos.Authentication.Response;
using portal_agile.Dtos.Modules;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Users.Response;
using portal_agile.Exceptions.Authentication;
using portal_agile.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace portal_agile.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthenticationService(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IConfiguration configuration,
            ILogger<AuthenticationService> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    throw new ArgumentException("Email and password are required");
                }

                // Get user by email
                var user = await _userRepository.GetUserByEmail(request.Email);
                if (user == null)
                {
                    throw new InvalidCredentialException(); // Don't reveal if email exists
                }

                // Check password using Identity's password verification
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (result.IsLockedOut)
                {
                    var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                    throw new UserLockedException(lockoutEnd?.DateTime ?? DateTime.UtcNow.AddMinutes(30), failedAttempts);
                }

                if (!result.Succeeded)
                {
                    throw new InvalidCredentialException(); // Generic message
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                var permissions = await _userRepository.GetAllUserPermissionsByUserId(user.Id);
                var modules = new List<Module>();

                // Generate tokens
                var accessToken = await GenerateAccessTokenAsync(user);
                var refreshToken = GenerateRefreshToken();
                var expiresIn = GetTokenExpirationSeconds();
                var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

                // Update user with refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 days for refresh token
                _userRepository.Update(user);

                _logger.LogInformation("User {Email} logged in successfully", request.Email);

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    TokenType = "Bearer",
                    ExpiresIn = expiresIn,
                    ExpiresAt = expiresAt,
                    User = new UserAuthResponseDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = roles,
                        // TODO: Fetch modules 
                        //Modules = modules =>
                    }
                };
            }
            catch (InvalidCredentialException)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
                throw; // Re-throw to be handled by controller
            }
            catch (UserLockedException ex)
            {
                _logger.LogWarning("Locked account login attempt for email: {Email}. Lockout until: {LockoutEnd}",
                    request.Email, ex.LockoutEnd);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", request.Email);
                throw;
            }
        }
        public  async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    throw new ArgumentException("Refresh token is required");
                }

                var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid refresh token");
                }

                // Generate new tokens
                var newAccessToken = await GenerateAccessTokenAsync(user);
                var newRefreshToken = GenerateRefreshToken();
                var expiresIn = GetTokenExpirationSeconds();
                var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Update refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                _userRepository.Update(user);

                return new LoginResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    TokenType = "Bearer",
                    ExpiresIn = expiresIn,
                    ExpiresAt = expiresAt,
                    User = new UserAuthResponseDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = roles.ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed");
                throw;
            }
        }
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                    return false;

                var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiry = DateTime.UtcNow;
                    _userRepository.Update(user);

                    _logger.LogInformation("User {Email} logged out successfully", user.Email);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return false;
            }
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return Task.FromResult(false);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        private async Task<string> GenerateAccessTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
            };

            // Add role claims using Identity
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add additional claims from Identity
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(GetTokenExpirationMinutes()),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private int GetTokenExpirationMinutes()
        {
            return int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        private int GetTokenExpirationSeconds()
        {
            return GetTokenExpirationMinutes() * 60;
        }
    }
}
