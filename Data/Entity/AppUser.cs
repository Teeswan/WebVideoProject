using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; } 
        public string? ProfileDescription { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? ProfileImagePath { get; set; }
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
        public ICollection<VideoLike> VideoLikes { get; set; } = new List<VideoLike>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
