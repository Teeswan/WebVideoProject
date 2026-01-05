using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data.Entity;
using System;

namespace Youtube_Entertainment_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistVideo> PlaylistVideos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<VideoTag> VideoTags { get; set; }
        public DbSet<VideoLike> VideoLikes { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().Property(u => u.Id).HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Channel>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Channels)
                .HasForeignKey(c => c.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Video>()
                .HasOne(v => v.Channel)
                .WithMany(c => c.Videos)
                .HasForeignKey(v => v.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Video>()
                .HasOne(v => v.Category)
                .WithMany(c => c.Videos)
                .HasForeignKey(v => v.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Thumbnail>()
                .HasOne(t => t.Video)
                .WithMany(v => v.Thumbnails)
                .HasForeignKey(t => t.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Video)
                .WithMany(v => v.Comments)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlaylistVideo>()
                .HasKey(pv => new { pv.PlaylistId, pv.VideoId });

            modelBuilder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.Playlist)
                .WithMany(p => p.Videos)
                .HasForeignKey(pv => pv.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.Video)
                .WithMany(v => v.PlaylistVideos)
                .HasForeignKey(pv => pv.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasKey(s => new { s.SubscriberUserId, s.ChannelOwnerUserId });

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Subscriber)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.SubscriberUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VideoLike>()
                .HasKey(vl => new { vl.UserId, vl.VideoId });

            modelBuilder.Entity<VideoLike>()
                .HasOne(vl => vl.User)
                .WithMany(u => u.VideoLikes)
                .HasForeignKey(vl => vl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VideoLike>()
                .HasOne(vl => vl.Video)
                .WithMany(v => v.Likes)
                .HasForeignKey(vl => vl.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VideoTag>()
                .HasKey(vt => new { vt.VideoId, vt.TagId });

            modelBuilder.Entity<VideoTag>()
                .HasOne(vt => vt.Video)
                .WithMany(v => v.VideoTags)
                .HasForeignKey(vt => vt.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VideoTag>()
                .HasOne(vt => vt.Tag)
                .WithMany(t => t.VideoTags)
                .HasForeignKey(vt => vt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryId)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.NotificationId);
                entity.HasOne(n => n.User)
                      .WithMany() 
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Channel>().Property(c => c.ChannelId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Video>().Property(v => v.VideoId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Comment>().Property(c => c.CommentId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Playlist>().Property(p => p.PlaylistId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Tag>().Property(t => t.TagId).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Thumbnail>().Property(t => t.ThumbnailId).HasDefaultValueSql("NEWID()");
        }
    }
}