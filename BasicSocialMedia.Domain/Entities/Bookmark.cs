namespace BasicSocialMedia.Domain.Entities
{
    public class Bookmark : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid TweetId { get; set; }
        public User User { get; set; }
        public Tweet Tweet { get; set; }
    }
}