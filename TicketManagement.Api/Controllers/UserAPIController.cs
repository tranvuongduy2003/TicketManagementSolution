using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly IUserService _userService;
        protected ResponseDto _response;

        public UserAPIController(IUserService userService)
        {
            _userService = userService;
            _response = new();
        }

        [Authorize]
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var userDtos = await _userService.GetUsers();

                _response.Data = userDtos;
                _response.Message = "Get users successfully!";
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
        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var userDto = await _userService.GetUserById(id);

                if (userDto is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy người dùng!";
                    return NotFound(_response);
                }

                _response.Data = userDto;
                _response.Message = "Get the user successfully!";
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var updatedUser = await _userService.UpdateUser(id, updateUserDto);

                if (updatedUser is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy người dùng!";
                    return NotFound(_response);
                }

                _response.Message = "Update the user successfully!";
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
        [HttpPatch("{id}/password")]
        public async Task<IActionResult> UpdateUserPassword(string id,
            [FromBody] UpdateUserPasswordDto updateUserPasswordDto)
        {
            try
            {
                var message = await _userService.UpdateUserPassword(id, updateUserPasswordDto);
                if (message is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy người dùng!";
                    return NotFound(_response);
                }

                if (message != "")
                {
                    _response.IsSuccess = false;
                    _response.Message = message;
                    return BadRequest(_response);
                }

                _response.Message = "Change user password successfully!";
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
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(string id,
            [FromBody] UpdateUserStatusDto updateUserStatusdDto)
        {
            try
            {
                var message = await _userService.UpdateUserStatus(id, updateUserStatusdDto);
                if (message is null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy người dùng!";
                    return NotFound(_response);
                }

                if (message != "")
                {
                    _response.IsSuccess = false;
                    _response.Message = message;
                    return BadRequest(_response);
                }

                _response.Message = "Change user status successfully!";
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