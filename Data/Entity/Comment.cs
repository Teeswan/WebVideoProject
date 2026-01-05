using System;
using System.Collections.Generic;

namespace Youtube_Entertainment_Project.Data.Entity
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public Guid VideoId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }

        // Navigation
        public Video? Video { get; set; } = null!;
        public AppUser? User { get; set; } = null!;

        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
