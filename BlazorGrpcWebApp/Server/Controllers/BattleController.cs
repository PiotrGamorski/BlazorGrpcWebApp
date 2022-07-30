using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BattleController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUtilityService _utilityService;
        private readonly IBattleService _battleService;

        public BattleController(DataContext dataContext, IUtilityService utilityService, IBattleService battleService)
        {
            _dataContext = dataContext;
            _utilityService = utilityService;
            _battleService = battleService;
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle([FromBody] int opponentId)
        {
            var attacker = await _utilityService.GetUser();
            var opponent = await _dataContext.Users.FindAsync(opponentId);
            if (opponent == null || opponent.IsDeleted)
                return NotFound("Opponent not available");

            var result = new BattleResult();
            await _battleService.Fight(_dataContext, attacker!, opponent, result);
            
            return Ok(result);
        }
    }
}
