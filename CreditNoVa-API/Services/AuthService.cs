using EFund_API.DataTransferObjects;
using EFund_API.Models;
using EFund_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using EFund_API.WebApp.Services.Services;
using EFund_API.WebApp.Services.Interfaces;

namespace EFund_API.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IRepository repo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
            : base(repo)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var users = await Repo.GetAllAsync<User>();
            var user = users.FirstOrDefault(u => u.Email == request.Email && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra email và username đã tồn tại
            var users = await Repo.GetAllAsync<User>();
            var existingUser = users.FirstOrDefault(u => u.Email == request.Email || u.Username == request.Username);

            if (existingUser != null)
                return null;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User",
                IsActive = true,
                IsEmailVerified = false,
                IsPhoneVerified = false,
                Address = request.Address,
                Gender = request.Gender,
            };

            await Repo.CreateAsync(user);
            await Repo.SaveAsync();

            return await GenerateAuthResponseAsync(user);
        }


        public Task<bool> LogoutAsync(string refreshToken)
        {
            // Không cần xử lý gì vì không dùng refresh token
            return Task.FromResult(true);
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
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

        public async Task<UserInfo?> GetUserInfoAsync(Guid userId)
        {
            var user = await Repo.GetByIdAsync<User>(userId);
            if (user == null) return null;

            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                Avatar = user.Avatar,
                Address = user.Address,
                Gender = user.Gender
            };
        }

        private Task<AuthResponse> GenerateAuthResponseAsync(User user)
        {
            var token = GenerateJwtToken(user);

            return Task.FromResult(new AuthResponse
            {
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified,
                    IsPhoneVerified = user.IsPhoneVerified,
                    Avatar = user.Avatar,
                    Address = user.Address,
                    Gender = user.Gender
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
