using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Follow;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicSocialMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost("{followingId:guid}")]
        [ProducesResponseType(200, Type = typeof(Result<FollowResponse>))]
        [ProducesResponseType(400, Type = typeof(Result<FollowResponse>))]
        public async Task<IActionResult> FollowUser(Guid followingId)
        {
            var result = await _followService.FollowUserAsync(followingId);
            if (result.Error != 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch("{followingId:guid}")]
        [ProducesResponseType(200, Type = typeof(Result<FollowResponse>))]
        [ProducesResponseType(400, Type = typeof(Result<FollowResponse>))]
        public async Task<IActionResult> UnfollowUser(Guid followingId)
        {
            var result = await _followService.UnfollowUserAsync(followingId);
            if (result.Error != 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{targetUserId:guid}/status")]
        [ProducesResponseType(200, Type = typeof(Result<FollowStatusResponse>))]
        [ProducesResponseType(400, Type = typeof(Result<FollowStatusResponse>))]
        public async Task<IActionResult> GetFollowStatus(Guid targetUserId)
        {
            var result = await _followService.GetFollowStatusAsync(targetUserId);
            if (result.Error != 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
