using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
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
        public async Task<IActionResult> Register(UserRegister request)
        {
            var response = await _authService.Register(new User() 
            {
                UserName = request.Username,
                Email = request.Email,
                Bananas = request.Bananas,
                DateOfBirth = request.DateOfBirth,
                IsConfirmed = request.IsConfirmed,
            }, request.Password);
            
            if(!response.Success)
                return BadRequest(response);
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
