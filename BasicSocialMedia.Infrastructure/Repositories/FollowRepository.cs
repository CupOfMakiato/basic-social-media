using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Domain.Entities;
using BasicSocialMedia.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BasicSocialMedia.Infrastructure.Repositories
{
    public class FollowRepository : GenericRepository<Follow>, IFollowRepository
    {
        public FollowRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimService claimsService)
            : base(context, timeService, claimsService)
        {
        }
        public async Task<List<Follow>> GetFollowingStatsAsync(Guid followerId, Guid followingId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(f => !f.IsDeleted && f.FollowerId == followerId && f.FollowingId == followingId)
                .ToListAsync();
        }

        public async Task<List<Follow>> GetFollowersAsync(Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(follow => follow.Follower)
                    .ThenInclude(user => user.ProfilePicture)
                .Where(follow =>
                    !follow.IsDeleted
                    && follow.FollowingId == userId
                    && !follow.Follower.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Follow>> GetFollowingAsync(Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(follow => follow.Following)
                    .ThenInclude(user => user.ProfilePicture)
                .Where(follow =>
                    !follow.IsDeleted
                    && follow.FollowerId == userId
                    && !follow.Following.IsDeleted)
                .ToListAsync();
        }
    }
}
