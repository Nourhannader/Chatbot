using System.Security.Claims;
using chatbot.Core.Common;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthService _authService;
        readonly IMailService _mailService;
        readonly ICurrentUser _currentUser;
        public AuthController(IAuthService authService, IMailService mailService, ICurrentUser currentUser)
        {
            this._authService = authService;
            _mailService = mailService;
            _currentUser = currentUser;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromForm] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model, GetIpAddress());
            
            var user = new EmailDto
            {
                FullName = $"{model.FirstName} {model.LastName}",
                Email = model.Email
            };
            await SendWelcomeEmailAsync(user);

            setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return StatusCode(
                 StatusCodes.Status201Created,
                 ApiResponse<object>.Ok(
                     new
                     {
                         result.AccessToken,
                         result.AccessTokenExpiresAt,
                         result.DeviceSessionId

                     },
                 "Account created successfully."));
        }

        
        [HttpPost("welcome")]
        [AllowAnonymous]
        public async Task<IActionResult> SendWelcomeEmail( EmailDto dto)
        {
            await SendWelcomeEmailAsync(dto);
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromForm] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.LoginAsync(model,GetIpAddress());
           
            if(!string.IsNullOrEmpty(result.RefreshToken))
                setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(
           ApiResponse<object>.Ok(
                     new
                     {
                         result.AccessToken,
                         result.AccessTokenExpiresAt,
                         result.DeviceSessionId

                     },
                   "Login successful."));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedException("Refresh token cookie not found.");
            }
            var result = await _authService.RefreshAsync(refreshToken,GetIpAddress());
            
            setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    result.AccessToken,
                    result.AccessTokenExpiresAt,
                    result.DeviceSessionId
                },
                "Token refreshed successfully."));
        }
        
        [HttpPost("Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> RevokeToken([FromBody] LogoutDto dto)
        {
            var token = dto.RefreshToken ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");
            await _authService.LogoutAsync(token,GetIpAddress());
            
            Response.Cookies.Delete("refreshToken");
            return Ok(
             ApiResponse<object>.Ok(
                     null!,
                     "Logged out successfully."));
        }

        //logout
        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = _currentUser.UserId;
            await _authService.LogoutAllAsync(userId, GetIpAddress());

            Response.Cookies.Delete("refreshToken");
            return Ok(
            ApiResponse<object>.Ok(
                    null!,
                    "Logged out from all devices successfully."));
        }


        private void setRefreshTokenInCookie(string refreshToken,DateTime expireOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expireOn,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path="api/auth"
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private string? GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString();
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

    }
}
