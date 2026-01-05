namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public Guid ChannelId { get; set; }
        public string Message { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public string? ActorProfileImage { get; set; } 
        public string? VideoThumbnail { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public AppUser User { get; set; } = null!;
    }
}
