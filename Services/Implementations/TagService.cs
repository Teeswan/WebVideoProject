using AutoMapper;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagService(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<List<TagDto>> GetAllTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return _mapper.Map<List<TagDto>>(tags);
        }

        public async Task<TagDto> GetTagByIdAsync(Guid id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) throw new Exception("Tag not found");
            return _mapper.Map<TagDto>(tag);
        }

        public async Task<TagDto> CreateTagAsync(TagDto dto)
        {
            var tag = _mapper.Map<Tag>(dto);
            await _tagRepository.AddAsync(tag);
            return _mapper.Map<TagDto>(tag);
        }

        public async Task<TagDto> UpdateTagAsync(Guid id, TagDto dto)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) throw new Exception("Tag not found");
            _mapper.Map(dto, tag);
            await _tagRepository.UpdateAsync(tag);
            return _mapper.Map<TagDto>(tag);
        }

        public async Task DeleteTagAsync(Guid id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) throw new Exception("Tag not found");
            await _tagRepository.DeleteAsync(tag);
        }
    }
}
