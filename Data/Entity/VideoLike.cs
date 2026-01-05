using System;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class VideoLike
    {
        public Guid UserId { get; set; }
        public Guid VideoId { get; set; }
        public bool IsLike { get; set; }
        public DateTime ReactedAt { get; set; }

        public AppUser User { get; set; } = null!;
        public Video Video { get; set; } = null!;
    }
}
