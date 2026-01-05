using AutoMapper;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<List<CommentDto>> GetAllCommentsAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            return _mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<CommentDto> GetCommentByIdAsync(Guid id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) throw new Exception("Comment not found");
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<CommentDto> CreateCommentAsync(CommentDto dto)
        {
            var comment = _mapper.Map<Comment>(dto);
            comment.CreatedAt = DateTime.UtcNow;
            await _commentRepository.AddAsync(comment);
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<CommentDto> UpdateCommentAsync(Guid id, CommentDto dto)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) throw new Exception("Comment not found");
            _mapper.Map(dto, comment);
            await _commentRepository.UpdateAsync(comment);
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) throw new Exception("Comment not found");
            await _commentRepository.DeleteAsync(comment);
        }
    }
}
