using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Enums;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Shared.gRPC_Services
{
    public class ArmyServiceGrpcImpl : ArmyServiceGrpc.ArmyServiceGrpcBase
    {
        private readonly DataContext _dataContext;
        public ArmyServiceGrpcImpl(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override async Task<GrpcHealUnitResponse> GrpcHealUnitGrpc(GrpcHealUnitRequest request, ServerCallContext context)
        {
            var authUser = await _dataContext.Users.FindAsync(request.AuthUserId);
            var userUnit = await _dataContext.UserUnits.FindAsync(request.UserUnitId);
            var unit = await _dataContext.Units.FindAsync(userUnit!.UnitId);

            var bananasCost = unit!.HitPoints - userUnit!.HitPoints;
            if (bananasCost > authUser!.Bananas)
            {
                return new GrpcHealUnitResponse()
                {
                    Success = false,
                    Message = $"Not enough bananas! You need { bananasCost } to heal this unit."
                };
            }

            bool unitAlreadyHealed = true;
            if (userUnit.HitPoints < userUnit.Unit.HitPoints)
            {
                unitAlreadyHealed = false;
                userUnit.HitPoints = userUnit!.Unit.HitPoints;
            }

            if (unitAlreadyHealed)
            {
                return new GrpcHealUnitResponse()
                {
                    Success = false,
                    Message = "Unit already healed."
                };
            }

            authUser.Bananas -= bananasCost;
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

            return new GrpcHealUnitResponse()
            {
                Success = true,
                Message = "Your unit has been healed.",
            };
        }

        public override async Task<GrpcReviveArmyResponse> GrpcReviveArmy(GrpcReviveArmyRequest request, ServerCallContext context)
        {
            var authUser = await _dataContext.Users.FindAsync(request.AuthUserId);
            var userUnits = await _dataContext.UserUnits
                .Where(u => u.UserId == authUser!.Id)
                .Include(u => u.Unit)
                .ToListAsync();

            int bananasCost = 1000;
            if (authUser!.Bananas < bananasCost)
            {
                return new GrpcReviveArmyResponse()
                {
                    Success = false,
                    Message = $"Not enough bananas! You need { bananasCost } to revive your army."
                };
            }

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
            {
                return new GrpcReviveArmyResponse()
                {
                    Success = false,
                    Message = "Your army is already alive.",
                };
            }

            authUser.Bananas -= bananasCost;
            await _dataContext.SaveChangesAsync();

            var reviveArmyActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == Activity.ReviveArmy))!.Id;
            var userLastActivity = new UserLastActivitie()
            {
                UserId = authUser.Id,
                ExecutionDate = DateTime.Now,
                LastActivityId = reviveArmyActivityId,
            };
            await _dataContext.UserLastActivities.AddAsync(userLastActivity);
            await _dataContext.SaveChangesAsync();

            return new GrpcReviveArmyResponse()
            {
                Success = true,
                Message = "Army revived!",
            };
        }
    }
}   
