namespace Youtube_Entertainment_Project.DTOs
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public Guid VideoId { get; set; }
        public Guid UserId { get; set; }  
        public string UserName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid? ParentCommentId { get; set; }
        public List<CommentDto>? Replies { get; set; }
        public string? UserProfilePictureUrl { get; set; } 

    }

}
