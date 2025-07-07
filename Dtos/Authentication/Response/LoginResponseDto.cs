using portal_agile.Dtos.Users.Response;
using portal_agile.Security;

namespace portal_agile.Dtos.Authentication.Response
{
    public class LoginResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }  // Optional but recommended
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }  // Seconds until expiration
        public DateTime ExpiresAt { get; set; }  // Absolute expiration time
        public required UserAuthResponseDto User { get; set; }  // User details
    }
}
