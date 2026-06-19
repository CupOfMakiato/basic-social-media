using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Domain.Entities;
using BasicSocialMedia.Infrastructure.Database;

namespace BasicSocialMedia.Infrastructure.Repositories
{
    public class TweetRepository : GenericRepository<Tweet>, ITweetRepository
    {
        public TweetRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimService claimsService)
            : base(context, timeService, claimsService)
        {
        }
    }
}
