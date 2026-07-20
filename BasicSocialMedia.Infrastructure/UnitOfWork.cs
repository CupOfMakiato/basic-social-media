using BasicSocialMedia.Application;
using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Infrastructure.Database;
namespace BasicSocialMedia.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IFollowRepository _followRepository;

        public UnitOfWork(AppDbContext dbContext,
            IUserRepository userRepository,
            IMediaRepository mediaRepository,
            IFollowRepository followRepository)

        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _followRepository = followRepository;
        }

        public IUserRepository UserRepository => _userRepository;
        public IMediaRepository MediaRepository => _mediaRepository;
        public IFollowRepository FollowRepository => _followRepository;
        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
