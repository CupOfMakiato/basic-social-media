using BasicSocialMedia.Application.Abstractions.RequestAndResponse.User;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicSocialMedia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserById()
        {
            var result = await _userService.GetCurrentUserById();
            if (result.Error != 0)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("profile-picture")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(Result<object>))]
        [ProducesResponseType(400, Type = typeof(Result<object>))]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] ProfilePictureUploadRequest request)
        {
            try
            {
                //var result = 
                await _userService.UploadProfilePicture(request.File);
                return Ok(new
                {
                    Error = 0,
                    Message = "Profile picture uploaded successfully!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = 1,
                    Message = $"An error occurred while uploading the profile picture: {ex.Message}"
                });

            }

        }
    }
}
