using BasicSocialMedia.Domain.Entities;
using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Follow;
using BasicSocialMedia.Application.Abstractions.Shared;

namespace BasicSocialMedia.Application.IServices
{
    public interface IFollowService
    {
        Task<List<Follow>> GetFollowingStatsAsync (Guid followerId, Guid followingId);
        Task<Result<FollowResponse>> FollowUserAsync(Guid followingId);
        Task<Result<FollowResponse>> UnfollowUserAsync(Guid followingId);
        Task<Result<FollowStatusResponse>> GetFollowStatusAsync(Guid targetUserId);
    }
}
