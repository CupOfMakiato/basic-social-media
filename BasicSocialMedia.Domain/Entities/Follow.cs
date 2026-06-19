using BasicSocialMedia.Domain.Enums;

namespace BasicSocialMedia.Domain.Entities
{
    public class Follow : BaseEntity
    {
        public Guid FollowingId { get; set; } // the user who we are following
        public Guid FollowerId { get; set; } // our followers
        public User Following { get; set; }
        public User Follower { get; set; }
    }
}