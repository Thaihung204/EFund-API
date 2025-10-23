using EFund_API.DataTransferObjects;

namespace EFund_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
        Task<UserInfo?> GetUserInfoAsync(Guid userId);
    }
}
