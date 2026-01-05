namespace Youtube_Entertainment_Project.DTOs
{
    public class SubscriptionDto
    {
        public Guid ChannelId { get; set; }
        public Guid SubscriberUserId { get; set; }
        public Guid ChannelOwnerUserId { get; set; }
        public DateTime SubscribedAt { get; set; }
        public string? ChannelName { get; set; }
        public string? ProfilePicture { get; set; }

    }
}
