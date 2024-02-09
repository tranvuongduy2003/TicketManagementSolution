using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Messaging.IMessaging;
using TicketManagement.Api.Models;
using Event = TicketManagement.Api.Models.Event;

namespace TicketManagement.Api.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentAPIController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        protected ResponseDto _response;

        public PaymentAPIController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _response = new();
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments([FromQuery] PaginationFilter filter)
        {
            try
            {
                var paymentObj = await _paymentService.GetPayments(filter);

                _response.Data = paymentObj.payments;
                _response.Meta = paymentObj.metadata;
                _response.Message = "Get payments successfully!";
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paymentObj.metadata));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPaymentsByUserId(string userId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var paymentObj = await _paymentService.GetPaymentsByUserId(userId, filter);

                _response.Data = paymentObj.payments;
                _response.Meta = paymentObj.metadata;
                _response.Message = "Get payments by user id successfully!";
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paymentObj.metadata));
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
        [Route("event/{eventId}")]
        public async Task<IActionResult> GetPaymentsByEventId(string eventId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var paymentObj = await _paymentService.GetPaymentsByEventId(eventId, filter);

                _response.Data = paymentObj.payments;
                _response.Meta = paymentObj.metadata;
                _response.Message = "Get payments by event id successfully!";
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paymentObj.metadata));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            try
            {
                var createdPayment = await _paymentService.Checkout(checkoutDto);

                _response.Data = createdPayment;
                _response.Message = "Checkout successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        // [Authorize]
        [HttpPost("create-stripe-session")]
        public async Task<IActionResult> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                var returnedStripeDto = _paymentService.CreateStripeSession(stripeRequestDto);

                _response.Data = returnedStripeDto;
                _response.Message = "Create stripe session successfully!";
            }
            catch (Exception ex)
            {
                _response.Data = ex.Message.ToString();
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        // [Authorize]
        [HttpPost("validate-stripe-session/{paymentId}")]
        public async Task<IActionResult> ValidateStripeSession(string paymentId)
        {
            try
            {
                var validateStripeResponseDto = _paymentService.ValidateStripeSession(paymentId);
                
                _response.Data = validateStripeResponseDto;
                _response.Message = "Validate stripe session successfully!";
            }
            catch (Exception ex)
            {
                _response.Data = ex.Message.ToString();
                _response.IsSuccess = false;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }
    }
}