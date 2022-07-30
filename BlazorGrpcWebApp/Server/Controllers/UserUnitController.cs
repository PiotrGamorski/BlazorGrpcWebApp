using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserUnitController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUtilityService _utilityService;

        public UserUnitController(DataContext dataContext, IUtilityService utilityService)
        {
            _dataContext = dataContext;
            _utilityService = utilityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserUnits()
        {
            var user = _utilityService.GetUser();
            var userUnits = await _dataContext.UserUnits.Where<UserUnit>(u => u.UserId == user.Result!.Id).ToListAsync();

            // TODO: use auto mapper?
            var userUnitsDtos = userUnits.Select(
                userUnit => new UserUnitDto()
                {
                    UnitId = userUnit.UnitId,
                    HitPoints = userUnit.HitPoints,
                });

            return Ok(userUnitsDtos);
        }

        [HttpPost]
        public async Task<IActionResult> BuildUserUnit([FromBody] int unitId)
        {
            var unit = await _dataContext.Units.FirstOrDefaultAsync<Unit>(u => u.Id == unitId);
            var user = _utilityService.GetUser();

            if (user.Result!.Bananas < unit!.BananaCost)
                return BadRequest("Not Enough Bananas");
            
            user.Result.Bananas -= unit!.BananaCost;
            var newUserUnit = new UserUnit()
            { 
                UnitId = unit.Id,
                UserId = user.Result.Id,
                HitPoints = unit.HitPoints,
            };
            await _dataContext.UserUnits.AddAsync(newUserUnit);
            await _dataContext.SaveChangesAsync();

            return Ok(newUserUnit);
        }

        [HttpDelete("{userUnitId}")]
        public async Task<IActionResult> DeleteUserUnit([FromRoute] int userUnitId)
        {
            var authUserId = _utilityService.GetUserUserId();
            var userUnit = await _dataContext.UserUnits
                .FirstOrDefaultAsync(u => u.Id == userUnitId && u.UserId == authUserId);

            if(userUnit == null)
                return NotFound();

            _dataContext.UserUnits.Remove(userUnit);
            await _dataContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("heal")]
        public async Task<ActionResult<GenericAuthResponse<UserUnit>>> HealUserUnit([FromBody] int userUnitId)
        {
            var authUser = await _utilityService.GetUser();
            var userUnit = await _dataContext.UserUnits.FindAsync(userUnitId);
            var unit = await _dataContext.Units.FindAsync(userUnit!.UnitId);

            var bananasCost = unit!.HitPoints - userUnit.HitPoints;
            if (bananasCost > authUser!.Bananas)
                return BadRequest($"Not enough bananas! You need {bananasCost} to heal this unit.");

            if (userUnit.HitPoints == unit.HitPoints)
                return Ok(new GenericAuthResponse<UserUnit>() { Message = "Unit already healed.", Success = false });
            else
            { 
                authUser.Bananas -= bananasCost;
                userUnit.HitPoints = userUnit.HitPoints;
                await _dataContext.SaveChangesAsync();

                return Ok(new GenericAuthResponse<UserUnit>() { Message = "Your unit has been healed.", Success = true });
            }
            
        }
    }
}
