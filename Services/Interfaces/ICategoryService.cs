using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        Task<CategoryDto> CreateCategoryAsync(CategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryDto dto);
        Task DeleteCategoryAsync(Guid id);
    }
}
