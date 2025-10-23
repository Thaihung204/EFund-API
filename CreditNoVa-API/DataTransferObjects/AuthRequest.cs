using System.ComponentModel.DataAnnotations;

namespace EFund_API.DataTransferObjects
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(6)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
    }

    //public class RefreshTokenRequest
    //{
    //    [Required]
    //    public string RefreshToken { get; set; }
    //}
}
