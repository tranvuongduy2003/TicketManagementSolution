using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [Authorize(Roles = "ADMIN")]
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
        
        [HttpGet("revenue")]
        [Authorize]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetRevenueStatistic()
        {
            try
            {
                var generalStatistic = await _statisticService.GetRevenueStatistic();
                
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
        
        [HttpGet("events-by-category")]
        [Authorize]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetEventsStatisticByCategory()
        {
            try
            {
                var eventDtos = await _statisticService.GetEventsStatisticByCategory();

                _response.Data = eventDtos;
                _response.Message = "Get events successfully!";
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
        [Authorize]
        [Authorize(Roles = "ADMIN")]
        [Route("event")]
        public async Task<IActionResult> GetEventsStatistic()
        {
            try
            {
                var eventsStatisticDto = await _statisticService.GetEventsStatistic();

                _response.Data = eventsStatisticDto;
                _response.Message = "Get events successfully!";
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
        [Authorize]
        [Authorize(Roles = "ADMIN, ORGANIZER")]
        [Route("event/{organizerId}")]
        public async Task<IActionResult> GetEventsStatisticByOrganizerId(string organizerId)
        {
            try
            {
                var eventsStatisticDto = await _statisticService.GetEventsStatisticByOrganizerId(organizerId);

                _response.Data = eventsStatisticDto;
                _response.Message = "Get events successfully!";
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
        [Authorize]
        [Authorize(Roles = "ADMIN")]
        [Route("payment/{eventId}")]
        public async Task<IActionResult> GetPaymentsStatisticByEventId(string eventId)
        {
            try
            {
                var eventsStatisticDto = await _statisticService.GetPaymentsStatisticByEventId(eventId);

                _response.Data = eventsStatisticDto;
                _response.Message = "Get events successfully!";
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