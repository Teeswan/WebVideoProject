using System;
using System.Collections.Generic;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Playlist
    {
        public Guid PlaylistId { get; set; }
        public Guid OwnerUserId { get; set; }
        public string Title { get; set; } = null!;
        public bool IsPublic { get; set; }

        // Navigation
        public AppUser Owner { get; set; } = null!;
        public ICollection<PlaylistVideo> Videos { get; set; } = new List<PlaylistVideo>();
    }

    public class PlaylistVideo
    {
        public Guid PlaylistId { get; set; }
        public Guid VideoId { get; set; }
        public int Position { get; set; }

        public Playlist Playlist { get; set; } = null!;
        public Video Video { get; set; } = null!;
    }
}
