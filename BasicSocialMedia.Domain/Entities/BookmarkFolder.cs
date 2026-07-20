namespace BasicSocialMedia.Domain.Entities
{
    public class BookmarkFolder : BaseEntity
    {
        public Guid OwnerId { get; set; }
        public User Owner { get; set; }
        public string Name { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    }
}