using BasicSocialMedia.Domain.Enums;

namespace BasicSocialMedia.Domain.Entities
{
    public class Block : BaseEntity
    {
        public Guid BlockedUserId { get; set; } // the user that got blocked
        public User BlockedUser { get; set; }
        public Guid BlockerId { get; set; } // us who did the block
        public User Blocker { get; set; } = null!;
    }
}