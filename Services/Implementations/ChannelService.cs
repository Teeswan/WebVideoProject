using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class ChannelService : IChannelService
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ChannelService(ApplicationDbContext context,IChannelRepository channelRepository, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _channelRepository = channelRepository;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<List<ChannelDto>> GetAllChannelsAsync()
        {
            var channels = await _channelRepository.GetAllAsync();
            return _mapper.Map<List<ChannelDto>>(channels);
        }

        public async Task<ChannelDto> GetChannelByIdAsync(Guid id)
        {
            var channel = await _context.Channels
                .Include(c => c.Owner)
                .Include(c => c.Subscribers)
                .Include(c => c.Videos)
                    .ThenInclude(v => v.Thumbnails)
                .FirstOrDefaultAsync(c => c.ChannelId == id);

            if (channel == null) throw new Exception("Channel not found");

            return _mapper.Map<ChannelDto>(channel);
        }
        public async Task<ChannelDto> GetChannelByUserIdAsync(Guid userId)
        {
            var channel = await _context.Channels
                .Include(c => c.Owner)
                .Include(c => c.Subscribers)
                .Include(c => c.Videos)
                .FirstOrDefaultAsync(c => c.OwnerUserId == userId);

            if (channel == null) return null;

            return _mapper.Map<ChannelDto>(channel);
        }

        public async Task<ChannelDto> CreateChannelAsync(ChannelDto dto, Guid ownerUserId)
        {
            var existingChannel = await GetChannelByUserIdAsync(ownerUserId);

            if (existingChannel != null)
                throw new Exception("This user already has a channel.");

            var channel = _mapper.Map<Channel>(dto);
            channel.OwnerUserId = ownerUserId;
            channel.CreatedAt = DateTime.UtcNow;

            await _channelRepository.AddAsync(channel);

            var user = await _userManager.FindByIdAsync(ownerUserId.ToString());

            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    var result = await _userManager.AddToRoleAsync(user, "Admin");

                    if (result.Succeeded)
                    {

                        await _userManager.UpdateSecurityStampAsync(user);
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                }
            }

            return _mapper.Map<ChannelDto>(channel);
        }

        public async Task<ChannelDto> UpdateChannelAsync(Guid id, ChannelDto dto, Guid currentUserId)
        {
            var channel = await _channelRepository.GetByIdAsync(id);
            if (channel == null) throw new Exception("Channel not found");
            if (channel.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You are not the owner of this channel.");

            _mapper.Map(dto, channel);
            await _channelRepository.UpdateAsync(channel);
            return _mapper.Map<ChannelDto>(channel);
        }

        public async Task DeleteChannelAsync(Guid id, Guid currentUserId)
        {
            var channel = await _channelRepository.GetByIdAsync(id);
            if (channel == null) throw new Exception("Channel not found");
            if (channel.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You are not the owner of this channel.");

            await _channelRepository.DeleteAsync(channel);
        }
        public async Task<bool> ChannelExistsByOwnerIdAsync(Guid ownerUserId)
        {
            return await _channelRepository.ExistsByOwnerIdAsync(ownerUserId);
        }
        public async Task<IEnumerable<ChannelDto>> SearchChannelsAsync(string searchTerm)
        {
            var searchTermLower = searchTerm.ToLower();

            var channels = await _context.Channels
                .Include(c => c.Owner) 
                .Include(c => c.Videos)
                    .ThenInclude(v => v.Thumbnails) 
                .Where(c => (c.ChannelName != null && c.ChannelName.ToLower().Contains(searchTermLower)) ||
                           (c.Name != null && c.Name.ToLower().Contains(searchTermLower)))
                .ToListAsync();

            return _mapper.Map<IEnumerable<ChannelDto>>(channels);
        }
    }
}
