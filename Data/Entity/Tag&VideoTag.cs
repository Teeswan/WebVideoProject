using System;
using System.Collections.Generic;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Tag
    {
        public Guid TagId { get; set; }
        public string Text { get; set; } = null!;
        public ICollection<VideoTag> VideoTags { get; set; } = new List<VideoTag>();
    }

    public class VideoTag
    {
        public Guid VideoId { get; set; }
        public Guid TagId { get; set; }

        public Video Video { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
