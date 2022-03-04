using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
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

        #region Simplifying Methods
        // based on Authorize attribute, one can read currently logged in user ID
        private int GetUserUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        private async Task<User?> GetUser() => await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == GetUserUserId());
        #endregion

        [HttpGet("getbananas")]
        public async Task<IActionResult> GetBananas()
        {
            var user = await GetUser();
            return Ok(user!.Bananas);
        }

        [HttpPut("addbananas")]
        public async Task<IActionResult> AddBananas([FromBody] int bananas)
        {
            var user = await GetUser();
            user!.Bananas += bananas;
            await _dataContext.SaveChangesAsync();
            return Ok(user.Bananas);
        }

        // this api is needed for BananaService when grpcMethod is used
        [HttpGet("getAuthUserId")]
        public Task<int> GetAuthorisedUserId()
        {
            var userId = GetUserUserId();
            return Task.FromResult(userId);
        }
    }
}
