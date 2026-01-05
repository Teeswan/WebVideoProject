namespace Youtube_Entertainment_Project.DTOs
{
    public class PlaylistDto
    {
        public Guid PlaylistId { get; set; }
        public string Title { get; set; } = null!;
        public Guid OwnerUserId { get; set; }
        public bool IsPublic { get; set; }

        public List<PlaylistVideoDto> Videos { get; set; } = new();
    }

    public class PlaylistVideoDto
    {
        public Guid VideoId { get; set; }
        public string VideoTitle { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public int Position { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
