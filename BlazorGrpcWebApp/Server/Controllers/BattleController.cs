using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;
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
        private readonly IBattleService _battleService;

        public BattleController(DataContext dataContext, IBattleService battleService)
        {
            _dataContext = dataContext;
            _battleService = battleService;
        }

        [HttpPost]
        public async Task<ActionResult<GenericAuthResponse<BattleResult>>> StartBattle([FromBody] StartBattleRequest request)
        {
            var attacker = await _dataContext.Users.FindAsync(request.AuthUserId);
            var opponent = await _dataContext.Users.FindAsync(request.OpponentId);

            if (opponent == null || opponent.IsDeleted)
                return NotFound(new GenericAuthResponse<BattleResult>() { Message = "Opponent not available", Success = false });

            var result = new BattleResult();
            await _battleService.Fight(_dataContext, attacker!, opponent, result);
            
            return Ok(new GenericAuthResponse<BattleResult>() { Data = result, Success = true});
        }
    }
}
