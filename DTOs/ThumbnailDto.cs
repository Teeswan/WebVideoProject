namespace Youtube_Entertainment_Project.DTOs
{
    public class ThumbnailDto
    {
        public Guid ThumbnailId { get; set; }
        public Guid VideoId { get; set; }
        public string Url { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}
