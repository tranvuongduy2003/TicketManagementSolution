using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Controllers
{
    [Route("event")]
    [ApiController]
    public class EventAPIController : ControllerBase
    {
        private readonly IEventService _eventService;
        protected ResponseDto _response;

        public EventAPIController(IEventService eventService)
        {
            _eventService = eventService;
            _response = new();
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] PaginationFilter filter)
        {
            try
            {
                var eventObj = await _eventService.GetEvents(filter);

                _response.Data = eventObj.events;
                _response.Meta = eventObj.metadata;
                _response.Message = "Get events successfully!";
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(eventObj.metadata));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpGet("statistic-by-category")]
        public async Task<IActionResult> GetEventsStatisticByCategory()
        {
            try
            {
                var eventDtos = await _eventService.GetEventsStatisticByCategory();

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
        
        [Authorize]
        [Authorize(Roles = "ORGANIZER")]
        [HttpGet]
        [Route("/event-by-organizer/{organizerId}")]
        public async Task<IActionResult> GetEventsByOrganizerId(string organizerId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var eventObj = await _eventService.GetEventsByOrganizerId(organizerId, filter);

                _response.Data = eventObj.events;
                _response.Meta = eventObj.metadata;
                _response.Message = "Get events successfully!";
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(eventObj.metadata));
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
        public async Task<IActionResult> GetEventById(string id)
        {
            try
            {
                var eventDto = await _eventService.GetEventById(id);

                if (eventDto is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy sự kiện!";
                    return NotFound(_response);
                }

                _response.Data = eventDto;
                _response.Message = "Get the event successfully!";
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
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            try
            {
                var eventDto = await _eventService.CreateEvent(createEventDto);

                _response.Data = eventDto;
                _response.Message = "Create the event successfully!";
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
        public async Task<IActionResult> UpdateEvent(string id, [FromBody] UpdateEventDto updateEventDto)
        {
            try
            {
                var result = await _eventService.UpdateEvent(id, updateEventDto);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy sự kiện!";
                    return NotFound(_response);
                }

                _response.Message = "Update the event successfully!";
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
        public async Task<IActionResult> DeleteEvent(string id)
        {
            try
            {
                var result = await _eventService.DeleteEvent(id);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy sự kiện!";
                    return NotFound(_response);
                }

                _response.Message = "Delete the event successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpPatch("/increase-favourite/{id}")]
        public async Task<IActionResult> IncreaseFavourite(string id)
        {
            try
            {
                var result = await _eventService.IncreaseFavourite(id);
                
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy sự kiện!";
                    return NotFound(_response);
                }

                _response.Data = result;
                _response.Message = "Create the event successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpPatch("/decrease-favourite/{id}")]
        public async Task<IActionResult> DecreaseFavourite(string id)
        {
            try
            {
                var result = await _eventService.DecreaseFavourite(id);
                
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy sự kiện!";
                    return NotFound(_response);
                }

                _response.Data = result;
                _response.Message = "Create the event successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpPatch("/increase-share/{id}")]
        public async Task<IActionResult> IncreaseShare(string id)
        {
            try
            {
                var result = await _eventService.IncreaseShare(id);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy sự kiện!";
                    return NotFound(_response);
                }

                _response.Data = result;
                _response.Message = "Create the event successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpPatch("/decrease-share/{id}")]
        public async Task<IActionResult> DecreaseShare(string id)
        {
            try
            {
                var result = await _eventService.DecreaseShare(id);

                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy sự kiện!";
                    return NotFound(_response);
                }

                _response.Data = result;
                _response.Message = "Create the event successfully!";
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