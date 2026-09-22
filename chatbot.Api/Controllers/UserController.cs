using System.Security.Claims;
using chatbot.Core.Common;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();

            var result =
                await _userService.GetProfileAsync(
                    userId);

            return Ok(
                ApiResponse<UserProfileDto>
                    .Ok(
                        result,
                        "Profile retrieved successfully."));
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
        [FromForm] UpdateUserDto dto)
        {
            var userId = GetCurrentUserId();

            var result =
                await _userService.UpdateProfileAsync(
                    userId,
                    dto);

            return Ok(
                ApiResponse<UserProfileDto>
                    .Ok(
                        result,
                        "Profile updated successfully."));
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedException("Invalid user identity.");
            }

            return userId;
        }
    }
}
