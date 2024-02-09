using Microsoft.AspNetCore.Mvc;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Controllers
{
    [Route("statistic")]
    [ApiController]
    public class StatisticAPIController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        protected ResponseDto _response;

        public StatisticAPIController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
            _response = new();
        }
        
        [HttpGet("general")]
        public async Task<IActionResult> GetGeneralStatistic()
        {
            try
            {
                var generalStatistic = await _statisticService.GetGeneralStatistic();
                
                _response.Data = generalStatistic;
                _response.Message = "Get statistics successfully!";
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