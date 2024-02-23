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
        public async Task<IActionResult> GetEvents([FromQuery] EventFilter filter)
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
        
        [HttpGet("newest")]
        public async Task<IActionResult> GetTop3NewestEvents()
        {
            try
            {
                var events = await _eventService.GetTop3NewestEvents();

                _response.Data = events;
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
        
        [HttpGet("upcoming")]
        public async Task<IActionResult> Get24hUpcomingEvents()
        {
            try
            {
                var events = await _eventService.Get24hUpcomingEvents();

                _response.Data = events;
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
        
        [HttpGet("highlight")]
        public async Task<IActionResult> GetHighlightEvent()
        {
            try
            {
                var highlightEventDto = await _eventService.GetHighlightEvent();

                _response.Data = highlightEventDto;
                _response.Message = "Get event successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpGet("highlight/list")]
        public async Task<IActionResult> GetTopHighlightEvents()
        {
            try
            {
                var highlightEventDto = await _eventService.GetHighlightEventsList();

                _response.Data = highlightEventDto;
                _response.Message = "Get event successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomEvents()
        {
            try
            {
                var events = await _eventService.GetRandomEvents();

                _response.Data = events;
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
        [Route("event-by-organizer/{organizerId}")]
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
        [Authorize]
        [Authorize(Roles = "ADMIN, ORGANIZER")]
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
        [Authorize]
        [Authorize(Roles = "ADMIN, ORGANIZER")]
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
        [Authorize]
        [Authorize(Roles = "ADMIN, ORGANIZER")]
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
        
        [HttpPatch("{id}/increase-favourite")]
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
        
        [HttpPatch("{id}/decrease-favourite")]
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
        
        [HttpPatch("{id}/increase-share")]
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
        
        [HttpPatch("{id}/decrease-share")]
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