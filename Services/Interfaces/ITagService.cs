using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagDto>> GetAllTagsAsync();
        Task<TagDto> GetTagByIdAsync(Guid id);
        Task<TagDto> CreateTagAsync(TagDto dto);
        Task<TagDto> UpdateTagAsync(Guid id, TagDto dto);
        Task DeleteTagAsync(Guid id);
    }
}
