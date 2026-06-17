using BasicSocialMedia.Application.DTOs.Auth;
using BasicSocialMedia.Domain.Entities;

namespace BasicSocialMedia.Application.IServices
{
    public interface IJwtTokenService
    {
        Task<JwtTokenResult> GenerateAndStoreTokensAsync(User user, string roleName);
        Task<bool> ValidateAccessTokenCacheAsync(string accessTokenId, string sessionId);
        Task<JwtTokenCacheItem?> GetSessionAsync(string sessionId);
        Task RevokeSessionAsync(string sessionId);
    }
}
