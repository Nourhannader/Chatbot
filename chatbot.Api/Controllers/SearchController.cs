using chatbot.Core.Common;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatbot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController(ISearchService searchService,ICurrentUser currentUser) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<SearchResultDto>>> Search([FromQuery] SearchRequestDto request)
        {
            var result = await searchService.SearchAsync(currentUser.UserId,request);

            return Ok(ApiResponse<PagedResultDto<SearchResultDto>>.Ok(result));
        }
    }
}
