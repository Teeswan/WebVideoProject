namespace Youtube_Entertainment_Project.DTOs
{
    public class UpdateVideoDto
    {
        public Guid VideoId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid>? TagIds { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
        public string Visibility { get; set; } = "public";
    }
}
