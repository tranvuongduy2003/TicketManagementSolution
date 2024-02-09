using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public async Task<ListCategoryObject> GetCategories(PaginationFilter filter)
    {
        try
        {
            var categories = _db.Categories.ToList();
            if (filter.search != null)
            {
                categories = categories.Where(c => EF.Functions.Contains(c.Name, filter.search)).ToList();
            } 
            categories = filter.order switch
            {
                PageOrder.ASC => categories.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => categories.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => categories
            };

            var metadata = new Metadata(categories.Count(), filter.page, filter.size);
            
            if (filter.takeAll == false)
            { 
                categories = categories.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return new ListCategoryObject
            {
                categories = categoryDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<CategoryDto?> GetCategoryById(string categoryId)
    {
        try
        {
            var category = _db.Categories.FirstOrDefault(t => t.Id == categoryId);

            if (category is null)
            {
                return null;
            }
            
            var categoryDto = _mapper.Map<CategoryDto>(category);

            return categoryDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        try
        {
            var category = _db.Categories.FirstOrDefault(c => c.Name == createCategoryDto.Name);

            if (category != null)
            {
                return false;
            }
            
            var newCategory = _mapper.Map<Category>(createCategoryDto);
            
            newCategory.CreatedAt = DateTime.Now;
            newCategory.UpdatedAt = DateTime.Now;
            
            _db.Categories.AddAsync(newCategory);
            _db.SaveChanges();
            
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> UpdateCategory(string categoryId, CreateCategoryDto updateCategoryDto)
    {
        try
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category is null)
            {
                return false;
            }

            category.Name = updateCategoryDto.Name;
            category.Color = updateCategoryDto.Color;
            category.UpdatedAt = DateTime.Now;
            
            _db.SaveChanges();
            
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> DeleteCategory(string categoryId)
    {
        try
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category is null)
            {
                return false;
            }

            _db.Categories.Remove(category);
            
            _db.SaveChanges();
            
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}