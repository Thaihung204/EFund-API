using EFund_API.DataTransferObjects;
using EFund_API.Dtos;

namespace EFund_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
        Task<UserInfo?> GetUserInfoAsync(Guid userId);
        Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(string email);
        Task<bool> DeactivateAccountAsync(Guid userId);
        Task<bool> ReactivateAccountAsync(Guid userId);

    }
}
