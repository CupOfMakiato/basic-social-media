using BasicSocialMedia.Domain.Entities;

namespace BasicSocialMedia.Application.IRepositories
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        Task<List<Follow>> GetFollowingStatsAsync(Guid followerId, Guid followingId);
        Task<List<Follow>> GetFollowersAsync(Guid userId);
        Task<List<Follow>> GetFollowingAsync(Guid userId);
    }
}
