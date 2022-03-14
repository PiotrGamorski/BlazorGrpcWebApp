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
    public class UserUnitController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUtilityService _utilityService;
        public UserUnitController(DataContext dataContext, IUtilityService utilityService)
        {
            _dataContext = dataContext;
            _utilityService = utilityService;
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

        [HttpGet]
        public async Task<IActionResult> GetUserUnits()
        { 
            var user = _utilityService.GetUser();
            var userUnits = await _dataContext.UserUnits.Where<UserUnit>(u => u.UserId == user.Result!.Id).ToListAsync();

            // The response represents a dto - "UserUnitResponse"
            var response = userUnits.Select(
                userUnit => new UserUnitResponse()
                {
                    UnitId = userUnit.UnitId,
                    HitPoints = userUnit.HitPoints,
                });

            return Ok(response);
        }
    }
}
