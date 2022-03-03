using BlazorGrpcWebApp.Shared.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public UserController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("getbananas")]
        public async Task<IActionResult> GetBananas()
        {
            // based on Authorize attribute, one can read currently logged in user ID
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(user!.Bananas);
        }
    }
}
