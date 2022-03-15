using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUtilityService _utilityService;
        public UserController(DataContext dataContext, IUtilityService utilityService)
        {
            _dataContext = dataContext;
            _utilityService = utilityService;
        }

        [HttpGet("getbananas")]
        public async Task<IActionResult> GetBananas()
        {
            var user = await _utilityService.GetUser();
            return Ok(user!.Bananas);
        }

        [HttpPut("addbananas")]
        public async Task<IActionResult> AddBananas([FromBody] int bananas)
        {
            var user = await _utilityService.GetUser();
            user!.Bananas += bananas;
            await _dataContext.SaveChangesAsync();
            return Ok(user.Bananas);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var users = await _dataContext.Users.Where(u => !u.IsDeleted && u.IsConfirmed).ToListAsync();
            users = users
                .OrderByDescending(u => u.Victories)
                .ThenBy(u => u.Defeats)
                .ThenBy(u => u.DateCreated)
                .ToList();

            int rank = 1;
            // Rank is computed based on upper order
            var response = users.Select(u => new UserStatistic()
            {
                Rank = rank++,
                UserId = u.Id,
                UserName = u.UserName,
                Battles = u.Battles,
                Victories = u.Victories,
                Defeats = u.Defeats,
            });

            return Ok(response);
        }

        // Api needed to get authorised user for gRPC services
        [HttpGet("getAuthUserId")]
        public Task<int> GetAuthorisedUserId()
        {
            var userId = _utilityService.GetUserUserId();
            return Task.FromResult(userId);
        }

        [HttpGet("getAuthUser")]
        public async Task<User> GetAuthUser()
        {
            var user = await _utilityService.GetUser();
            return user!;
        }
    }
}
