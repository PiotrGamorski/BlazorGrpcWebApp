using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Enums;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Services;
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

            var buildUnitActivity = new Activity();
            switch (unit!.Title)
            {
                case "Knight":
                    buildUnitActivity = Activity.BuildKnight;
                    break;
                case "Archer":
                    buildUnitActivity = Activity.BuildArcher;
                    break;
                case "Mage":
                    buildUnitActivity = Activity.BuildMage;
                    break;
                default: break;
            }
            var buildUnitActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == buildUnitActivity))!.Id;
            var userLastActivity = new UserLastActivitie()
            {
                UserId = user.Id,
                ExecutionDate = DateTime.Now,
                LastActivityId = buildUnitActivityId,
            };
            await _dataContext.UserLastActivities.AddAsync(userLastActivity);
            await _dataContext.SaveChangesAsync();

            await DeleteActivityService.DeleteOldestActivity(_dataContext, user.Id, ActivitySimplified.Build);

            return Ok(newUserUnit);
        }

        [HttpDelete("{userUnitId}")]
        public async Task<IActionResult> DeleteUserUnit([FromRoute] int userUnitId)
        {
            var authUserId = _utilityService.GetUserUserId();
            var authUser = await _dataContext.Users.FindAsync(authUserId);
            var userUnit = await _dataContext.UserUnits
                .FirstOrDefaultAsync(u => u.Id == userUnitId && u.UserId == authUserId);
            var unit = await _dataContext.Units.FirstOrDefaultAsync(u => u.Id == userUnit!.UnitId);
            var bananasReward = userUnit!.HitPoints;

            if(userUnit == null)
                return NotFound();

            _dataContext.UserUnits.Remove(userUnit);
            authUser!.Bananas += bananasReward;
            await _dataContext.SaveChangesAsync();

            var deleteUnitActivity = new Activity();
            switch (unit!.Title)
            {
                case "Knight":
                    deleteUnitActivity = Activity.DeleteKnight;
                    break;
                case "Archer":
                    deleteUnitActivity = Activity.DeleteArcher;
                    break;
                case "Mage":
                    deleteUnitActivity = Activity.DeleteMage;
                    break;
                default: break;
            }
            var deleteUnitActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == deleteUnitActivity))!.Id;
            var userLastActivity = new UserLastActivitie()
            {
                UserId = authUser.Id,
                ExecutionDate = DateTime.Now,
                LastActivityId = deleteUnitActivityId,
            };
            await _dataContext.UserLastActivities.AddAsync(userLastActivity);
            await _dataContext.SaveChangesAsync();

            await DeleteActivityService.DeleteOldestActivity(_dataContext, authUser.Id, ActivitySimplified.Delete);

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

                var healUnitActivity = new Activity();
                switch (unit.Title)
                {
                    case "Knight":
                        healUnitActivity = Activity.HealKnight;
                        break;
                    case "Archer":
                        healUnitActivity = Activity.HealArcher;
                        break;
                    case "Mage":
                        healUnitActivity = Activity.HealMage;
                        break;
                    default: break;
                }
                var healUnitActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == healUnitActivity))!.Id;
                var userLastActivity = new UserLastActivitie()
                {
                    UserId = authUser.Id,
                    ExecutionDate = DateTime.Now,
                    LastActivityId = healUnitActivityId,
                };
                await _dataContext.UserLastActivities.AddAsync(userLastActivity);
                await _dataContext.SaveChangesAsync();

                await DeleteActivityService.DeleteOldestActivity(_dataContext, authUser.Id, ActivitySimplified.Heal);

                return Ok(new GenericAuthResponse<UserUnit>() { Message = "Your unit has been healed.", Success = true });
            }
        }

        [HttpGet("reviveUserUnits")]
        public async Task<IActionResult> ReviveUserUnits()
        {
            var user = await _utilityService.GetUser();
            var userUnits = await _dataContext.UserUnits
                .Where(u => u.UserId == user!.Id)
                .Include(u => u.Unit)
                .ToArrayAsync();

            int bananasCost = 1000;
            if (user!.Bananas < bananasCost)
                return BadRequest($"Not enough bananas! You need {bananasCost} to revive your army.");

            bool armyAlreadyAlive = true;
            foreach (var userUnit in userUnits)
            {
                if (userUnit.HitPoints <= 0)
                {
                    armyAlreadyAlive = false;
                    userUnit.HitPoints = new Random().Next(0, userUnit.Unit.HitPoints);
                }
            }

            if (armyAlreadyAlive)
                return Ok("Your army is already alive.");

            user.Bananas -= bananasCost;
            await _dataContext.SaveChangesAsync();

            var reviveArmyActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == Activity.ReviveArmy))!.Id;
            var userLastActivity = new UserLastActivitie()
            {
                UserId = user.Id,
                ExecutionDate = DateTime.Now,
                LastActivityId = reviveArmyActivityId,
            };
            await _dataContext.UserLastActivities.AddAsync(userLastActivity);
            await _dataContext.SaveChangesAsync();

            await DeleteActivityService.DeleteOldestActivity(_dataContext, user.Id, Activity.ReviveArmy);

            return Ok("Army revived!");
        }
    }
}
