using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterRequestDto request)
        {
            var response = await _authService.Register(new User() 
            {
                UserName = request.UserRegister.Username,
                Email = request.UserRegister.Email,
                Bananas = request.UserRegister.Bananas,
                DateOfBirth = request.UserRegister.DateOfBirth,
                IsConfirmed = request.UserRegister.IsConfirmed,
            }, 
            request.UserRegister.Password,
            request.StartUnitId);

            if (response.Success)
                return Ok(response);
            return BadRequest(response);
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLogin request)
        { 
            var response = await _authService.Login(request.Email, request.Password);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
           
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify([FromBody]VerifyCodeRequestDto request)
        {
            var response = await _authService.Verify(request);
            if (response.Success)
            { 
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }

        [HttpGet("userEmailExists")]
        [AllowAnonymous]
        public async Task<IActionResult> UserEmailExists([FromQuery] string email)
        { 
            var response = await _authService.UserEmailExists(email);
            return Ok(response);
        }

        [HttpGet("userNameExists")]
        [AllowAnonymous]
        public async Task<IActionResult> UserNameExists([FromQuery] string userName)
        {
            var response = await _authService.UserNameExists(userName);
            return Ok(response);
        }
    }
}
