using System;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Subscription
    {
        public Guid SubscriberUserId { get; set; }
        public Guid ChannelOwnerUserId { get; set; }
        public DateTime SubscribedAt { get; set; }
        public AppUser Subscriber { get; set; } = null!;
    }
}
