using AutoMapper;
using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
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
        private readonly IMapper _mapper;

        public UserController(DataContext dataContext, IUtilityService utilityService, IMapper mapper)
        {
            _dataContext = dataContext;
            _utilityService = utilityService;
            _mapper = mapper;
        }

        [HttpGet("{authUserId}/getbananas")]
        public async Task<ActionResult<GenericAuthResponse<int>>> GetBananas([FromQuery] int authUserId)
        {
            var authUser = await _dataContext.Users.FindAsync(authUserId);
            if (authUser == null) 
                return NotFound(new GenericAuthResponse<int>() {  Success = false, Message = StatusCodes.Status404NotFound.ToString() });
            else 
                return Ok(new GenericAuthResponse<int>() { Data = authUser.Bananas, Success = true});
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
            var response = users.Select(user => 
            {
                var userLeaderBoardEntry = _mapper.Map<UserLeaderboardEntry>(user);
                userLeaderBoardEntry.Rank = rank++;

                return userLeaderBoardEntry;
            });

            return Ok(response);
        }

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
