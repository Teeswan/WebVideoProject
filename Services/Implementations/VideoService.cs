using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public VideoService(IVideoRepository videoRepository, IChannelRepository channelRepository, IMapper mapper, ApplicationDbContext context)
        {
            _videoRepository = videoRepository;
            _channelRepository = channelRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<VideoDto>> GetAllVideosAsync()
        {
            var videos = await _context.Videos
                .Include(v => v.Channel)
                    .ThenInclude(c => c.Owner)
                .Include(v => v.Thumbnails)
                .ToListAsync();

            return _mapper.Map<List<VideoDto>>(videos);
        }

        public async Task<VideoDto> GetVideoByIdAsync(Guid id)
        {
            var video = await _context.Videos
                .Include(v => v.Channel)
                    .ThenInclude(c => c.Owner)
                .Include(v => v.Thumbnails)
                .Include(v => v.VideoTags)
                    .ThenInclude(vt => vt.Tag)
                .FirstOrDefaultAsync(v => v.VideoId == id);

            if (video == null) throw new Exception("Video not found");
            return _mapper.Map<VideoDto>(video);
        }

        public async Task<VideoDto> CreateVideoAsync(CreateVideoDto dto, Guid currentUserId)
        {
            var channel = await _channelRepository.GetByIdAsync(dto.ChannelId);
            if (channel == null) throw new Exception("Channel does not exist.");
            if (channel.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You can only upload videos to your own channel.");

            var video = _mapper.Map<Video>(dto);

            // We let EF generate the ID or set it manually here
            if (video.VideoId == Guid.Empty) video.VideoId = Guid.NewGuid();

            video.ChannelId = dto.ChannelId;
            video.UploadTime = DateTime.UtcNow;
            video.DurationSeconds = video.DurationSeconds == 0 ? 1 : video.DurationSeconds;
            video.FilePath = dto.FilePath ?? throw new Exception("FilePath is required for video upload.");

            if (dto.TagIds != null)
            {
                video.VideoTags = dto.TagIds.Select(tagId => new VideoTag
                {
                    VideoId = video.VideoId,
                    TagId = tagId
                }).ToList();
            }

            await _videoRepository.AddAsync(video);

            // Return the DTO (The Controller can get the ID from result.VideoId)
            return _mapper.Map<VideoDto>(video);
        }

        public async Task<VideoDto> UpdateVideoAsync(Guid id, UpdateVideoDto dto, Guid currentUserId)
        {
            var video = await _context.Videos
                .Include(v => v.Channel)
                .Include(v => v.VideoTags)
                .FirstOrDefaultAsync(v => v.VideoId == id);

            if (video == null) throw new Exception("Video not found");
            if (video.Channel == null) throw new Exception("Video channel not loaded.");
            if (video.Channel.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You can only edit videos in your own channel.");

            _mapper.Map(dto, video);

            if (dto.TagIds != null)
            {
                video.VideoTags.Clear();
                video.VideoTags = dto.TagIds.Select(tagId => new VideoTag
                {
                    VideoId = video.VideoId,
                    TagId = tagId
                }).ToList();
            }

            await _videoRepository.UpdateAsync(video);
            return _mapper.Map<VideoDto>(video);
        }

        public async Task DeleteVideoAsync(Guid id, Guid currentUserId)
        {
            var video = await _context.Videos
                .Include(v => v.Channel)
                .FirstOrDefaultAsync(v => v.VideoId == id);

            if (video == null) throw new Exception("Video not found");
            if (video.Channel == null) throw new Exception("Video channel not loaded.");
            if (video.Channel.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You can only delete videos in your own channel.");

            await _videoRepository.DeleteAsync(video);
        }

        public async Task UpdateVideoViewsAsync(Guid videoId, int newViewCount)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video != null)
            {
                video.ViewCount = newViewCount;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasUserLikedAsync(Guid videoId, Guid userId)
        {
            return await _context.VideoLikes.AnyAsync(vl => vl.VideoId == videoId && vl.UserId == userId);
        }

        public async Task<bool> ToggleLikeAsync(Guid videoId, Guid userId)
        {
            var existing = await _context.VideoLikes
                .FirstOrDefaultAsync(vl => vl.VideoId == videoId && vl.UserId == userId);

            if (existing != null)
            {
                _context.VideoLikes.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }
            else
            {
                _context.VideoLikes.Add(new VideoLike { VideoId = videoId, UserId = userId });
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<int> GetLikeCountAsync(Guid videoId)
        {
            return await _context.VideoLikes.CountAsync(vl => vl.VideoId == videoId);
        }

        public async Task<long> GetTotalLikesForChannelAsync(Guid channelId)
        {
            var videoIds = await _context.Videos
                .Where(v => v.ChannelId == channelId)
                .Select(v => v.VideoId)
                .ToListAsync();

            if (!videoIds.Any()) return 0;

            return await _context.VideoLikes
                .Where(vl => videoIds.Contains(vl.VideoId))
                .LongCountAsync();
        }
        public async Task<IEnumerable<VideoDto>> SearchVideosAsync(string searchTerm, Guid? currentUserId)
        {
            var searchTermLower = searchTerm.ToLower();

            var videos = await _context.Videos
                .Include(v => v.Thumbnails)
                .Include(v => v.Channel) 
                .Where(v => v.Title.ToLower().Contains(searchTermLower) ||
                           (v.Description != null && v.Description.ToLower().Contains(searchTermLower)))
                .Where(v => v.Visibility == "public" ||
                           (v.Channel != null && v.Channel.OwnerUserId == currentUserId))
                .ToListAsync();

            return _mapper.Map<IEnumerable<VideoDto>>(videos);
        }
    }
}