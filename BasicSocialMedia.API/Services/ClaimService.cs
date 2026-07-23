using BasicSocialMedia.Application.IServices;
using System.Security.Claims;

namespace BasicSocialMedia.API.Services
{
    public class ClaimService : IClaimService
    {
        public ClaimService(IHttpContextAccessor httpContextAccessor)
        {
            var id = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContextAccessor.HttpContext?.User?.FindFirstValue("id");

            GetCurrentUserId = Guid.TryParse(id, out var userId) ? userId : Guid.Empty;
        }

        public Guid GetCurrentUserId { get; }
    }
}
