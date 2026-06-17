using BasicSocialMedia.Application.IServices;

namespace BasicSocialMedia.Application.Services
{
    public class CurrentTime : ICurrentTime
    {
        public DateTime GetCurrentTime() => DateTime.UtcNow;
    }
}
