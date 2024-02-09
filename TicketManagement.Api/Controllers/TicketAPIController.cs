using Microsoft.AspNetCore.Mvc;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Controllers
{
    [Route("ticket")]
    [ApiController]
    public class TicketAPIController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        protected ResponseDto _response;

        public TicketAPIController(ITicketService ticketService)
        {
            _ticketService = ticketService;
            _response = new();
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] PaginationFilter filter)
        {
            try
            {
                var ticketObj = await _ticketService.GetTickets(filter);

                _response.Data = ticketObj.Tickets;
                _response.Meta = ticketObj.Metadata;
                _response.Message = "Get tickets successfully!";
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
        [Route("my-tickets/{userId}")]
        public async Task<IActionResult> GetTicketsByUserId(string userId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var ticketObj = await _ticketService.GetTicketsByUserId(userId, filter);

                _response.Data = ticketObj.Tickets;
                _response.Meta = ticketObj.Metadata;
                _response.Message = "Get tickets successfully!";
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
        public async Task<IActionResult> GetTicketById(string id)
        {
            try
            {
                var ticketDto = await _ticketService.GetTicketById(id);

                if (ticketDto is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy thông tin vé!";
                    return NotFound(_response);
                }

                _response.Data = ticketDto;
                _response.Message = "Get the ticket successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTicketInfo(string id, [FromBody] UdpateTicketDto udpateTicketDto)
        {
            try
            {
                var ticketDto = await _ticketService.UpdateTicketInfo(id, udpateTicketDto);

                if (ticketDto is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy thông tin vé!";
                    return NotFound(_response);
                }

                _response.Data = ticketDto;
                _response.Message = "Update the ticket successfully!";
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
        [Route("get-by-payment/{paymentId}")]
        public async Task<IActionResult> GetTicketsByPaymentId(string paymentId)
        {
            try
            {
                var ticketDtos = await _ticketService.GetTicketsByPaymentId(paymentId);

                _response.Data = ticketDtos;
                _response.Message = "Get tickets successfully!";
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
        [Route("create-tickets/{paymentId}")]
        public async Task<IActionResult> CreateTickets(string paymentId, [FromBody] CreateTicketsDto createTicketsDto)
        {
            try
            {
                var ticketDtos = await _ticketService.CreateTickets(paymentId, createTicketsDto);

                _response.Data = ticketDtos;
                _response.Message = "Create new tickets successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpPatch]
        [Route("validate-tickets/{paymentId}")]
        public async Task<IActionResult> ValidateTickets(string paymentId, [FromBody] bool isSuccess)
        {
            try
            {
                var ticketDtos = await _ticketService.ValidateTickets(paymentId, isSuccess);

                _response.Data = ticketDtos;
                _response.Message = "Validate tickets successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
        
        [HttpPatch]
        [Route("terminate/{ticketId}")]
        public async Task<IActionResult> TerminateTicket(string ticketId)
        {
            try
            {
                var ticketDtos = await _ticketService.TerminateTicket(ticketId);

                if (ticketDtos == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy thông tin vé!";
                    return NotFound(_response);
                }

                _response.Data = ticketDtos;
                _response.Message = "Terminate new tickets successfully!";
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