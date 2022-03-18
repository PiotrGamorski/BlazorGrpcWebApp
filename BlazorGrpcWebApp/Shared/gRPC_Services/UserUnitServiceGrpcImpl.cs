using Grpc.Core;
using BlazorGrpcWebApp.Shared.Data;
using Microsoft.EntityFrameworkCore;
using BlazorGrpcWebApp.Shared.Entities;

namespace BlazorGrpcWebApp.Shared.gRPC_Services
{
    public class UserUnitServiceGrpcImpl : UserUnitServiceGrpc.UserUnitServiceGrpcBase
    {
        private readonly DataContext _dataContext;
        public UserUnitServiceGrpcImpl(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override async Task<GrpcUserUnitResponse> BuildUserUnit(GrpcUserUnitRequest request, ServerCallContext context)
        {
            var unit = await _dataContext.Units.FirstOrDefaultAsync<Unit>(u => u.Id == request.UnitId);
            var user  = await _dataContext.Users.FirstOrDefaultAsync<User>(u => u.Id == request.UserId);

            if (request.Bananas < unit!.BananaCost)
                throw new RpcException(new Status(StatusCode.Unavailable, "Not Enough Bananas"));
            
            user!.Bananas -= unit.BananaCost;
            var newUserUnit = new UserUnit()
            {
                UnitId = unit.Id,
                UserId = request.UserId,
                HitPoints = unit.HitPoints,
            };
            await _dataContext.UserUnits.AddAsync(newUserUnit);
            await _dataContext.SaveChangesAsync();

            return new GrpcUserUnitResponse()
            {
                UnitId = unit.Id,
                UserId = request.UserId,
                HitPoints = unit.HitPoints,
                Success = true,
                Message = $"Your {unit.Title} has been built!"
            };
        }

        public override async Task GetUserUnits(GrpcGetUserUnitRequest request, IServerStreamWriter<GrpcGetUserUnitResponse> responseStream, ServerCallContext context)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            var userUnits = await _dataContext.UserUnits.Where<UserUnit>(u => u.UserId == user!.Id).ToListAsync();

            if (userUnits != null && userUnits.Any())
            {
                foreach (var userUnit in userUnits)
                {
                    await responseStream.WriteAsync(new GrpcGetUserUnitResponse()
                    {
                        UserunitId = userUnit.Id,
                        UnitId = userUnit.UnitId,
                        HitPoints = userUnit.HitPoints,
                    });
                }
            }
            else await responseStream.WriteAsync(new GrpcGetUserUnitResponse());
        }
    }
}
