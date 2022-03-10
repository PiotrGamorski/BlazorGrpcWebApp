using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // this api is needed for BananaService when grpcMethod is used
        [HttpGet("getAuthUserId")]
        public Task<int> GetAuthorisedUserId()
        {
            var userId = _utilityService.GetUserUserId();
            return Task.FromResult(userId);
        }
    }
}
