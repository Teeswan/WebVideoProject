using System;
using System.Collections.Generic;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Channel
    {
        public Guid ChannelId { get; set; }
        public string? ChannelName { get; set; }
        public Guid OwnerUserId { get; set; }
        public AppUser Owner { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Video> Videos { get; set; } = new List<Video>();
        public ICollection<Subscription> Subscribers { get; set; } = new List<Subscription>();
    }
}
