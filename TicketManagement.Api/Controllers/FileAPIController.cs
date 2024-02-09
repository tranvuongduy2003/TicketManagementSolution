using Microsoft.AspNetCore.Mvc;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Services;

namespace TicketManagement.Api.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileAPIController : ControllerBase
    {
        private readonly FileService _fileService;
        protected ResponseDto _response;

        public FileAPIController(FileService fileService)
        {
            _fileService = fileService;
            _response = new();
        }

        [HttpGet]
        public async Task<IActionResult> ListAllBlobs()
        {
            try
            {
                var result = await _fileService.ListAsync();

                _response.Data = result;
                _response.Message = "Get files list successfully!";
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
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var result = await _fileService.UploadAsync(file);

                _response.Data = result;
                _response.Message = "Upload file successfully!";
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
        [Route("filename")]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {
                var result = await _fileService.DownloadAsync(filename);

                return File(result.Content, result.ContentType, result.Name);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
        
        [HttpDelete]
        [Route("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            try
            {
                var result = await _fileService.DeleteAsync(filename);

                _response.Data = result;
                _response.Message = "Delete file successfully!";
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