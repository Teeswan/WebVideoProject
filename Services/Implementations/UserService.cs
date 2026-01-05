using AutoMapper;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(UserDto dto)
        {
            var appuser = _mapper.Map<AppUser>(dto);
            appuser.CreatedAt = DateTime.UtcNow;
            await _userRepository.AddAsync(appuser);
            return _mapper.Map<UserDto>(appuser);
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            _mapper.Map(dto, user);
            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            await _userRepository.DeleteAsync(user);
        }
    }
}
