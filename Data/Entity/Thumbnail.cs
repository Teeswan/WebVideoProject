using System;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Thumbnail
    {
        public Guid ThumbnailId { get; set; }
        public Guid VideoId { get; set; }
        public string Url { get; set; } = null!;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsDefault { get; set; }

        // Navigation
        public Video Video { get; set; } = null!;
    }
}
