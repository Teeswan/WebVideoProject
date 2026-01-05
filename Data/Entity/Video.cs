using System;
using System.Collections.Generic;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Video
    {
        public Guid VideoId { get; set; }
        public Guid ChannelId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Visibility { get; set; } = "public";
        public Guid? CategoryId { get; set; }
        public int DurationSeconds { get; set; }
        public DateTime UploadTime { get; set; }
        public string? FilePath { get; set; }


        // Counters
        public long ViewCount { get; set; }
        public long LikeCount { get; set; }
        public long DislikeCount { get; set; }
        public long CommentCount { get; set; }

        // Navigation
        public Channel Channel { get; set; } = null!;
        public Category? Category { get; set; }
        public ICollection<Thumbnail> Thumbnails { get; set; } = new List<Thumbnail>();
        public ICollection<VideoTag> VideoTags { get; set; } = new List<VideoTag>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<VideoLike> Likes { get; set; } = new List<VideoLike>();
        public ICollection<PlaylistVideo> PlaylistVideos { get; set; } = new List<PlaylistVideo>();
    }
}
