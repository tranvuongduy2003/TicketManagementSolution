using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface ICategoryService
{
    Task<ListCategoryObject> GetCategories(PaginationFilter? filter);
    Task<CategoryDto?> GetCategoryById(string categoryId);
    Task<bool> CreateCategory(CreateCategoryDto createCategoryDto);
    Task<bool> UpdateCategory(string categoryId, CreateCategoryDto updateCategoryDto);
    Task<bool> DeleteCategory(string categoryId);
}