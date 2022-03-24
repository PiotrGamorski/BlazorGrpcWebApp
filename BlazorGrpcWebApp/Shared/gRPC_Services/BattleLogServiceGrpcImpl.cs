using BlazorGrpcWebApp.Shared.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Shared.gRPC_Services
{
    public class BattleLogServiceGrpcImpl : BattleLogServiceGrpc.BattleLogServiceGrpcBase
    {
        private readonly DataContext _dataContext;

        public BattleLogServiceGrpcImpl(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override async Task<GrpcShowBattleLogsResponse> GrpcShowBattleLogs(GrpcShowBattleLogsRequest request, ServerCallContext context)
        {
            var showBattleLogs = await _dataContext.BattleLogs
                .AnyAsync(b => b.AttackerId == request.AuthUserId && b.OpponentId == request.OppenentId);

            return new GrpcShowBattleLogsResponse() { Show = showBattleLogs };
        }

        public override async Task GrpcGetBattleLogs(GrpcGetBattleLogsRequest request, IServerStreamWriter<GrpcGetBattlelogsResponse> responseStream, ServerCallContext context)
        {
            var BattleLogs = await _dataContext.BattleLogs
                .Where(b => b.AttackerId == request.AuthUserId && b.OpponentId == request.OppenentId)
                .ToListAsync();

            if(BattleLogs != null && BattleLogs.Any())
            {
                foreach (var battleLog in BattleLogs)
                {
                    await responseStream.WriteAsync(new GrpcGetBattlelogsResponse()
                    {
                        Log = battleLog.Log
                    });
                }
            }
        }
    }
}
