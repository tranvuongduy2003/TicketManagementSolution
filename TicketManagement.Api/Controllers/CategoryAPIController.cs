using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Controllers
{
    [Route("category")]
    [ApiController]
    public class CategoryAPIController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        protected ResponseDto _response;

        public CategoryAPIController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
            _response = new();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] PaginationFilter filter)
        {
            try
            {
                var categoryObj = await _categoryService.GetCategories(filter);

                _response.Data = categoryObj.categories;
                _response.Meta = categoryObj.metadata;
                _response.Message = "Get categories successfully!";
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(categoryObj.metadata));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpGet("statistic")]
        [Authorize]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetStatisticCategories([FromQuery] PaginationFilter filter)
        {
            try
            {
                var categoryObj = await _categoryService.GetStatisticCategories(filter);

                _response.Data = categoryObj.categories;
                _response.Meta = categoryObj.metadata;
                _response.Message = "Get statistic categories successfully!";
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(categoryObj.metadata));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            try
            {
                var categoryDto = await _categoryService.GetCategoryById(id);

                if (categoryDto is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy thể loại!";
                    return NotFound(_response);
                }

                _response.Data = categoryDto;
                _response.Message = "Get the category successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            try
            {
                var categoryDto = await _categoryService.CreateCategory(createCategoryDto);

                _response.Data = categoryDto;
                _response.Message = "Create the category successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            try
            {
                var result = await _categoryService.UpdateCategory(id, updateCategoryDto);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy thể loại!";
                    return NotFound(_response);
                }

                _response.Message = "Update the category successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy thể loại!";
                    return NotFound(_response);
                }

                _response.Message = "Delete the category successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
    }
}