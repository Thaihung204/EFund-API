using EFund_API.DataTransferObjects;
using EFund_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFund_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest(new { message = "Email hoặc username đã tồn tại" });

            return Ok(result);
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var success = await _authService.LogoutAsync("");
            return Ok(new { message = "Đăng xuất thành công" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserInfo>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var userInfo = await _authService.GetUserInfoAsync(userId);
            if (userInfo == null)
                return NotFound(new { message = "Không tìm thấy thông tin người dùng" });

            return Ok(userInfo);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
        {
            var isValid = await _authService.ValidateTokenAsync(request.Token);
            return Ok(new { isValid });
        }
    }

    public class ValidateTokenRequest
    {
        public string Token { get; set; }
    }
}
