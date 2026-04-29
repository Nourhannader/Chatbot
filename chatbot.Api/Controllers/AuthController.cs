using chatbot.Core.DTOs.Auth;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthService _authService;
        readonly IMailService _mailService;
        public AuthController(IAuthService authService, IMailService mailService)
        {
            this._authService = authService;
            _mailService = mailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result);

            var user = new EmailDto
            {
                FullName = $"{model.FirstName} {model.LastName}",
                Email = model.Email
            };
            await SendWelcomeEmailAsync(user);

            setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        private async Task SendWelcomeEmailAsync(EmailDto user)
        {
            var filePath = $"{Directory.GetCurrentDirectory()}\\templates\\welcomeEmail.html";
            var str = new StreamReader(filePath);
            var mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[fullName]", $"{user.FullName}")
                .Replace("[email]", user.Email);
            await _mailService.SendEmailAsync(user.Email, "Welcome to NexTalk", mailText);
        }

        
        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeEmail( EmailDto dto)
        {
            var filePath=$"{Directory.GetCurrentDirectory()}\\templates\\welcomeEmail.html";
            var str = new StreamReader(filePath);
            var mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[fullName]", $"{dto.FullName}")
                .Replace("[email]", dto.Email);
            await _mailService.SendEmailAsync(dto.Email, "Welcome to NexTalk", mailText);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result);
            if(!string.IsNullOrEmpty(result.RefreshToken))
                setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token is missing!");
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
                return BadRequest(result);
            setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");
            var result = await _authService.RevokeTokenAsync(token);
            if (!result)
                return NotFound("Token Invalid!");
            Response.Cookies.Delete("refreshToken");
            return Ok("Token revoked successfully!");
        }

        private void setRefreshTokenInCookie(string refreshToken,DateTime expireOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expireOn.ToLocalTime(),
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Secure = true

            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }
}
