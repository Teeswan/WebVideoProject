using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<UserDto> CreateUserAsync(UserDto dto);
        Task<UserDto> UpdateUserAsync(Guid id, UserDto dto);
        Task DeleteUserAsync(Guid id);
    }
}
