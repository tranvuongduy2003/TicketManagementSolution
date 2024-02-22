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

            var metadata = new Metadata(categories.Count(), filter.page, filter.size, filter.takeAll);

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

    public async Task<ListCategoryObject> GetStatisticCategories(PaginationFilter filter)
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

            var metadata = new Metadata(categories.Count(), filter.page, filter.size, filter.takeAll);

            if (filter.takeAll == false)
            {
                categories = categories.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var categoryDtos = (from c in categories
                    join e in _db.Events on c.Id equals e.CategoryId into joinedCategories
                    from cj in joinedCategories.DefaultIfEmpty()
                    select new
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Color = c.Color,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        EventId = cj != null ? cj.Id : null,
                    })
                .GroupBy(cj => new
                {
                    Id = cj.Id,
                    Name = cj.Name,
                    Color = cj.Color,
                    CreatedAt = cj.CreatedAt,
                    UpdatedAt = cj.UpdatedAt,
                }).Select(cg => new
                    CategoryDto
                    {
                        Id = cg.Key.Id,
                        Name = cg.Key.Name,
                        Color = cg.Key.Color,
                        CreatedAt = cg.Key.CreatedAt,
                        UpdatedAt = cg.Key.UpdatedAt,
                        TotalEvents = cg.Count(cgi => cgi.EventId != null),
                        TotalTickets = cg.Sum(cgi =>
                            cgi.EventId != null
                                ? _db.Payments.Where(p => p.EventId == cgi.EventId).Sum(p => p.Quantity)
                                : 0)
                    });

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