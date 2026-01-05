using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllCommentsAsync();
        Task<CommentDto> GetCommentByIdAsync(Guid id);
        Task<CommentDto> CreateCommentAsync(CommentDto dto);
        Task<CommentDto> UpdateCommentAsync(Guid id, CommentDto dto);
        Task DeleteCommentAsync(Guid id);
    }
}
