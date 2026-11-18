using System.Security.Claims;
using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Exceptions;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Models;
using chatbot.Ef.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IUserService userService,ISearchService searchService,ICurrentUser currentUser) : ControllerBase
    {
        

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
        {
            var userId = User.GetUserId();

            var result = await userService.GetProfileAsync(userId);

            return Ok(
                ApiResponse<UserProfileDto>
                    .Ok(result,"Profile retrieved successfully."));
        }

        [HttpPut("me")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromForm] UpdateUserDto dto)
        {
            var userId = User.GetUserId();

            var result =
                await userService.UpdateProfileAsync(userId, dto);

            return Ok(
                ApiResponse<UserProfileDto>
                    .Ok(result,"Profile updated successfully."));
        }

        [HttpPut("image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<StoredFile>> UpdateProfileImage([FromForm] UpdateProfileImageDto dto)
        {
            var userId = User.GetUserId();

            var result =await userService.UpdateProfileImageAsync(userId,dto.Image);

            return Ok(
               ApiResponse<StoredFile>
                   .Ok(result, "Image Profile updated successfully."));
        }


        [HttpDelete("image")]
        public async Task<IActionResult>DeleteProfileImage()
        {
            var userId = User.GetUserId();

            await userService.DeleteProfileImageAsync( userId);

            return NoContent();
        }


    }
}
