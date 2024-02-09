using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Messaging.IMessaging;

namespace TicketManagement.Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _configuration = configuration;
            _response = new();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            try
            {
                var errorMessage = await _authService.Register(model);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    _response.IsSuccess = false;
                    _response.Message = errorMessage;
                    return BadRequest(_response);
                }

                await _messageBus.PublishMessage(model.Email,
                    _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                var loginResponse = await _authService.Login(model);
                if (loginResponse.User == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Tài khoản hoặc mật khẩu không đúng";
                    return Unauthorized(_response);
                }

                _response.Data = loginResponse;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto tokenRequestDto)
        {
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                if (tokenRequestDto is null || tokenRequestDto.RefreshToken is null ||
                    tokenRequestDto.RefreshToken == "" ||
                    accessToken is null || accessToken == "")
                {
                    _response.IsSuccess = false;
                    _response.Message = "Thông tin xác thực không hợp lệ";
                    return BadRequest(_response);
                }


                var newAccessToken = await _authService.RefreshToken(accessToken, tokenRequestDto.RefreshToken);

                if (newAccessToken == "" || newAccessToken is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Thông tin xác thực không hợp lệ";
                    return BadRequest(_response);
                }

                _response.Data = newAccessToken;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

            return Ok(_response);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                var user = await _authService.GetUserProfile(accessToken);
                if (user is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Thông tin xác thực không hợp lệ";
                    return Unauthorized(_response);
                }

                _response.Data = user;
                _response.Message = "Get user profile successfully";
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