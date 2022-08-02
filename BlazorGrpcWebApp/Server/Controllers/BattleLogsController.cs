using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BattleLogsController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public BattleLogsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost("show")]
        public async Task<ActionResult<GenericAuthResponse<bool>>> ShowBattleLogs([FromBody] ShowBattleLogsRequest request)
        {
            try
            {
                var result = await _dataContext.BattleLogs
                .AnyAsync(b => b.AttackerId == request.AuthUserId && b.OpponentId == request.OpponentId);

                return Ok(new GenericAuthResponse<bool>() { Data = result, Success = true });
            }
            catch (Exception e)
            {
                return BadRequest(new GenericAuthResponse<bool>() { Success = false, Message = e.Message });
            }
        }

        [HttpPost("getLeaderboard")]
        public async Task<ActionResult<GenericAuthResponse<List<BattleLog>>>> GetBattleLogs([FromBody] GetBattleLogsRequest request)
        {
            try
            {
                var result = await _dataContext.BattleLogs
                    .Where(b => b.AttackerId == request.AuthUserId && b.OpponentId == request.OpponentId)
                    .ToListAsync();

                return Ok(new GenericAuthResponse<List<BattleLog>>() { Data = result, Success = true});
            }
            catch (Exception e)
            {
                return BadRequest(new GenericAuthResponse<List<BattleLog>>() { Success = false, Message = e.Message });
            }
        }
    }
}
