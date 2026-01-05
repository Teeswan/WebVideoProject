namespace Youtube_Entertainment_Project.DTOs
{
    public class VideoDto
    {
        public Guid VideoId { get; set; }
        public string Visibility { get; set; } = "public";
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ChannelOwnerUserId { get; set; }
        public string? ChannelOwnerProfileImage { get; set; }
        public string? ChannelName { get; set; }
        public Guid ChannelId { get; set; }
        public Guid? CategoryId { get; set; }
        public long ViewCount { get; set; }
        public long LikeCount { get; set; }
        public long CommentCount { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public List<string> Tags { get; set; } = new List<string>();
        public string? FilePath { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSubscribedByCurrentUser { get; set; }
        public int SubscriberCount { get; set; }
        public string? ThumbnailUrl { get; set; }

    }
}