using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
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

            if (!response.Success)
            {
                return BadRequest(response);
            }
                
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin request)
        { 
            var response = await _authService.Login(request.Email, request.Password);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
