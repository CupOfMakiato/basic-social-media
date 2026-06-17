using BasicSocialMedia.Domain.Entities;
using BasicSocialMedia.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BasicSocialMedia.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }

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
                entity.HasKey(user => user.Id);
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

                entity.HasData(
                   new User
                   {
                       // admin@example.com
                       // @Admin123
                       Id = new Guid("9a6c1f69-6df7-4dc3-8f34-d6495a2cb001"),
                       RoleId = 1,
                       UserName = "admin",
                       Email = "v1:YQvFiXrtZ6pNFPnT:+SxlOXcfm3iCRVuHXMxp/w==:5GYYX5znCTRit7wGOa+B/xM=",
                       Password = "$2a$12$iqMkHQW3cQz.FqVLTuyPaeG5kWnGCxP1qjx73Ut/S7L1HJ7QEkWIa",
                       Status = UserStatus.Active,
                       CreationDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                       IsDeleted = false
                   },
                   new User
                   {
                       // user@example.com
                       // @User123
                       Id = new Guid("f604b6a6-5096-4738-8cda-eab8c0b17002"),
                       RoleId = 2,
                       UserName = "user",
                       Email = "v1:+Y444vlUQjwQ9QXL:9NpcWXZunoQNIMFBVyhC0g==:xtGNAb22AECjaDLbu1bj6Q==",
                       Password = "$2a$12$wsRAixvGaai/Wbi.YwSovuPZ5sinV98cj12Fwjxxj5AC/gmF7/IkW",
                       Status = UserStatus.Active,
                       CreationDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                       IsDeleted = false
                   });
            });
        }
    }
}
