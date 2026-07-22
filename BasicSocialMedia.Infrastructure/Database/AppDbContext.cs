using BasicSocialMedia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasicSocialMedia.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Tweet> Tweet { get; set; }
        public DbSet<Hashtag> Hashtag { get; set; }
        public DbSet<TweetHashtag> TweetHashtag { get; set; }
        public DbSet<Bookmark> Bookmark { get; set; }
        public DbSet<BookmarkFolder> BookmarkFolder { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<Block> Block { get; set; }
        public DbSet<Follow> Follow { get; set; }
        public DbSet<DirectMessageChat> DirectMessageChat { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Notification> Notification { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(role => role.Id);
                entity.Property(role => role.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasIndex(role => role.RoleName)
                    .IsUnique();

                entity.HasData(
                    new Role { Id = 1, RoleName = "Admin" },
                    new Role { Id = 2, RoleName = "User" });
            });

            modelBuilder.Entity<User>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property(user => user.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(user => user.Email)
                    .IsRequired()
                    .HasMaxLength(512);
                entity.Property(user => user.Password)
                    .IsRequired();
                entity.HasIndex(user => user.UserName)
                    .IsUnique();
                entity.HasIndex(user => user.Email)
                    .IsUnique();
                entity.HasOne(user => user.Role)
                    .WithMany(role => role.Users)
                    .HasForeignKey(user => user.RoleId);
                entity.HasOne(user => user.ProfilePicture)
                    .WithOne(media => media.User)
                    .HasForeignKey<Media>(media => media.UserProfilePictureId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.Navigation(user => user.ProfilePicture)
                    .IsRequired(false);
                entity.Property(user => user.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

            });

            modelBuilder.Entity<Tweet>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property(tweet => tweet.Content)
                    .IsRequired()
                    .HasMaxLength(300);
                entity.Property(tweet => tweet.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.HasOne(tweet => tweet.TweetAuthor)
                    .WithMany()
                    .HasForeignKey(tweet => tweet.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tweet => tweet.ParentTweet)
                    .WithMany(tweet => tweet.Replies)
                    .HasForeignKey(tweet => tweet.ParentTweetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tweet => tweet.QuoteTweet)
                    .WithMany()
                    .HasForeignKey(tweet => tweet.QuoteTweetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(tweet => tweet.Media)
                    .WithOne(media => media.Tweet)
                    .HasForeignKey(media => media.TweetId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(tweet => tweet.Bookmarks)
                    .WithOne(bookmark => bookmark.Tweet)
                    .HasForeignKey(bookmark => bookmark.TweetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(tweet => tweet.LikedByUsers)
                    .WithOne(like => like.Tweet)
                    .HasForeignKey(like => like.TweetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Ignore(tweet => tweet.BookmarkedByUsers);
            });

            modelBuilder.Entity<Hashtag>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property(hashtag => hashtag.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(hashtag => hashtag.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<TweetHashtag>(entity =>
            {
                entity.HasKey(tweetHashtag => new
                {
                    tweetHashtag.TweetId,
                    tweetHashtag.HashtagId
                });

                entity.HasOne(tweetHashtag => tweetHashtag.Tweet)
                    .WithMany(tweet => tweet.TweetHashtags)
                    .HasForeignKey(tweetHashtag => tweetHashtag.TweetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tweetHashtag => tweetHashtag.Hashtag)
                    .WithMany()
                    .HasForeignKey(tweetHashtag => tweetHashtag.HashtagId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bookmark>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property<Guid?>("BookmarkFolderId");

                entity.HasIndex(bookmark => new
                {
                    bookmark.UserId,
                    bookmark.TweetId
                }).IsUnique();

                entity.HasOne(bookmark => bookmark.User)
                    .WithMany()
                    .HasForeignKey(bookmark => bookmark.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bookmark => bookmark.Tweet)
                    .WithMany(tweet => tweet.Bookmarks)
                    .HasForeignKey(bookmark => bookmark.TweetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BookmarkFolder>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property(folder => folder.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(folder => new
                {
                    folder.OwnerId,
                    folder.Name
                }).IsUnique();

                entity.HasOne(folder => folder.Owner)
                    .WithMany()
                    .HasForeignKey(folder => folder.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(folder => folder.Bookmarks)
                    .WithOne()
                    .HasForeignKey("BookmarkFolderId")
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(like => new
                {
                    like.UserId,
                    like.TweetId
                });

                entity.HasOne(like => like.User)
                    .WithMany()
                    .HasForeignKey(like => like.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(like => like.Tweet)
                    .WithMany(tweet => tweet.LikedByUsers)
                    .HasForeignKey(like => like.TweetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Block>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.HasIndex(block => new
                {
                    block.BlockerId,
                    block.BlockedUserId
                }).IsUnique();

                entity.HasOne(block => block.Blocker)
                    .WithMany()
                    .HasForeignKey(block => block.BlockerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(block => block.BlockedUser)
                    .WithMany()
                    .HasForeignKey(block => block.BlockedUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.HasIndex(follow => new
                {
                    follow.FollowerId,
                    follow.FollowingId
                }).IsUnique();

                entity.HasOne(follow => follow.Follower)
                    .WithMany()
                    .HasForeignKey(follow => follow.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(follow => follow.Following)
                    .WithMany()
                    .HasForeignKey(follow => follow.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DirectMessageChat>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.HasIndex(chat => new
                {
                    chat.ParticipantOneId,
                    chat.ParticipantTwoId
                }).IsUnique();

                entity.HasOne(chat => chat.ParticipantOne)
                    .WithMany()
                    .HasForeignKey(chat => chat.ParticipantOneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(chat => chat.ParticipantTwo)
                    .WithMany()
                    .HasForeignKey(chat => chat.ParticipantTwoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(chat => chat.Messages)
                    .WithOne(message => message.DirectMessageChat)
                    .HasForeignKey(message => message.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property(message => message.MessageContent)
                    .HasMaxLength(2000);
                entity.Property(message => message.SentAt)
                    .HasDefaultValueSql("now()");

                entity.HasOne(message => message.DirectMessageChat)
                    .WithMany(chat => chat.Messages)
                    .HasForeignKey(message => message.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(message => message.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(message => message.Media)
                    .WithOne(media => media.Message)
                    .HasForeignKey(media => media.MessageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Media>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property(media => media.FileName)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(media => media.FileUrl)
                    .IsRequired()
                    .HasMaxLength(2048);
                entity.Property(media => media.FileType)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(media => media.FilePublicId)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(media => media.UserProfilePictureId)
                    .IsRequired(false);

                entity.HasOne(media => media.Tweet)
                    .WithMany(tweet => tweet.Media)
                    .HasForeignKey(media => media.TweetId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(media => media.Message)
                    .WithMany(message => message.Media)
                    .HasForeignKey(media => media.MessageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                ConfigureBaseEntity(entity);

                entity.Property(notification => notification.NotificationContent)
                    .IsRequired()
                    .HasMaxLength(500);
            });
        }

        private static void ConfigureBaseEntity<TEntity>(EntityTypeBuilder<TEntity> entity)
            where TEntity : BaseEntity
        {
            entity.HasKey(model => model.Id);

            entity.Property(model => model.CreationDate)
                .HasDefaultValueSql("now()");
            entity.Property(model => model.IsDeleted)
                .HasDefaultValue(false);

            entity.HasIndex(model => model.IsDeleted);
            entity.HasIndex(model => model.CreatedBy);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(model => model.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(model => model.ModificationBy)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(model => model.DeleteBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
