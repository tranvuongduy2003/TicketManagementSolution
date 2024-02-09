using Microsoft.AspNetCore.Mvc;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Controllers
{
    [Route("email")]
    [ApiController]
    public class EmailAPIController : ControllerBase
    {
        private readonly ISendService _sendService;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        public EmailAPIController(ISendService sendService,IConfiguration configuration)
        {
            _sendService = sendService;
            _configuration = configuration;
            _response = new();
        }


        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] EmailRequestDto model)
        {
            try
            {
                await _sendService.SendEmail(model.Email, model.Title, model.Message);
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