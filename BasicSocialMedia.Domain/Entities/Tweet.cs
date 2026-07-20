using BasicSocialMedia.Domain.Enums;

namespace BasicSocialMedia.Domain.Entities
{
    public class Tweet : BaseEntity
    {
        public string Content { get; set; }
        public TweetStatus Status { get; set; } = TweetStatus.Draft;
        // List of images
        public ICollection<Media> Media { get; set; } = new List<Media>();
        // Tweet can have many bookmarks
        public ICollection<Bookmark> BookmarkedByUsers { get; set; }
        // Tweet can have many likes
        public ICollection<Like> LikedByUsers { get; set; }
        public Guid AuthorId { get; set; }
        public User TweetAuthor { get; set; }
        // nullable, if the tweet is a reply it will has parent tweet, if not it will be null
        public Guid? ParentTweetId { get; set; }
        public Tweet? ParentTweet { get; set; }
        public ICollection<Tweet> Replies { get; set; } = new List<Tweet>();
        public Guid? QuoteTweetId { get; set; }
        public Tweet? QuoteTweet { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<TweetHashtag> TweetHashtags { get; set; } = new List<TweetHashtag>();
    }
}
