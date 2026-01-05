using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public PlaylistService(IPlaylistRepository playlistRepository, IMapper mapper, ApplicationDbContext context)
        {
            _playlistRepository = playlistRepository;
            _mapper = mapper;
            _context = context;
            
        }

        public async Task<List<PlaylistDto>> GetAllPlaylistsAsync()
        {
            var playlists = await _context.Playlists
                .Include(p => p.Videos)
                    .ThenInclude(pv => pv.Video)
                        .ThenInclude(v => v.Thumbnails)
                .ToListAsync();

            return _mapper.Map<List<PlaylistDto>>(playlists);
        }

        public async Task<PlaylistDto> GetPlaylistByIdAsync(Guid id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Videos)
                    .ThenInclude(pv => pv.Video)
                        .ThenInclude(v => v.Thumbnails)
                .FirstOrDefaultAsync(p => p.PlaylistId == id);

            if (playlist == null) throw new Exception("Playlist not found");

            return _mapper.Map<PlaylistDto>(playlist);
        }

        public async Task<PlaylistDto> CreatePlaylistAsync(PlaylistDto dto)
        {
            var playlist = _mapper.Map<Playlist>(dto);
            await _playlistRepository.AddAsync(playlist);
            return _mapper.Map<PlaylistDto>(playlist);
        }

        public async Task<PlaylistDto> UpdatePlaylistAsync(Guid id, PlaylistDto dto)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist == null) throw new Exception("Playlist not found");
            _mapper.Map(dto, playlist);
            await _playlistRepository.UpdateAsync(playlist);
            return _mapper.Map<PlaylistDto>(playlist);
        }

        public async Task DeletePlaylistAsync(Guid id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist == null) throw new Exception("Playlist not found");
            await _playlistRepository.DeleteAsync(playlist);
        }
        public async Task AddVideoToPlaylistAsync(Guid playlistId, Guid videoId)
        {
            var playlist = await _playlistRepository.GetByIdAsync(playlistId);
            if (playlist == null) throw new Exception("Playlist not found");

            var exists = playlist.Videos.Any(v => v.VideoId == videoId);
            if (exists) return;

            int nextPosition = playlist.Videos.Count + 1;

            var playlistVideo = new PlaylistVideo
            {
                PlaylistId = playlistId,
                VideoId = videoId,
                Position = nextPosition
            };

            playlist.Videos.Add(playlistVideo);
            await _playlistRepository.UpdateAsync(playlist);
        }
        public async Task<IEnumerable<PlaylistDto>> GetPlaylistsByUserIdAsync(Guid userId)
        {
            var playlists = await _context.Playlists
                .Where(p => p.OwnerUserId == userId)
                .Include(p => p.Videos)
                    .ThenInclude(pv => pv.Video)
                        .ThenInclude(v => v.Thumbnails)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PlaylistDto>>(playlists);
        }
    }
}
