namespace Youtube_Entertainment_Project.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalVideos { get; set; }
        public int TotalUsers { get; set; }
        public int TotalChannels { get; set; }
        public long TotalViews { get; set; }
        public long TotalLikes { get; set; }
        public List<VideoDto> RecentVideos { get; set; }
    }
}
