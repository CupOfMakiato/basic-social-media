using BasicSocialMedia.Application;
using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Infrastructure.Database;
namespace BasicSocialMedia.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public UnitOfWork(AppDbContext dbContext,
            IUserRepository userRepository)

        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public IUserRepository UserRepository => _userRepository;
        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
