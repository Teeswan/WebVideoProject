using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.DTOs
{
    public class ChannelDto
    {
        public Guid ChannelId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid OwnerUserId { get; set; }
        public string? OwnerProfileImagePath { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<VideoDto> Videos { get; set; } = new List<VideoDto>();
        public List<SubscriptionDto> Subscribers { get; set; } = new List<SubscriptionDto>();
    }
}
