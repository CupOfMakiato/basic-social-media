using BasicSocialMedia.Application.IRepositories;

namespace BasicSocialMedia.Application
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public IMediaRepository MediaRepository { get; }
        //public IAuthRepository AuthRepository { get; }
        public Task<int> SaveChangeAsync();
    }
}
