using BlazorGrpcWebApp.Shared.Data;
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

        public override async Task<GrpcReviveArmyResponse> GrpcReviveArmy(GrpcReviveArmyRequest request, ServerCallContext context)
        {
            var authUser = await _dataContext.Users.FindAsync(request.AuthUserId);
            var userUnits = await _dataContext.UserUnits
                .Where(u => u.UserId == authUser!.Id)
                .Include(u => u.Unit)
                .ToListAsync();

            int bananasCost = 1000;
            if (authUser!.Bananas < bananasCost)
                return new GrpcReviveArmyResponse()
                {
                    Success = false,
                    Message = $"Not enough bananas! You need { bananasCost } to revive your army."
                };

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
                return new GrpcReviveArmyResponse()
                {
                    Success = false,
                    Message = "Your army is already alive.",
                };

            authUser.Bananas -= bananasCost;
            await _dataContext.SaveChangesAsync();
            return new GrpcReviveArmyResponse()
            {
                Success = true,
                Message = "Army revived!",
            };
        }
    }
}   
