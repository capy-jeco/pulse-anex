using portal_agile.Dtos.Authentication.Requests;
using portal_agile.Dtos.Authentication.Response;

namespace portal_agile.Contracts.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
    }
}
