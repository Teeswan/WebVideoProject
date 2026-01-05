namespace Youtube_Entertainment_Project.DTOs
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public string Message { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public string? ActorProfileImage { get; set; }
        public string? VideoThumbnail { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}