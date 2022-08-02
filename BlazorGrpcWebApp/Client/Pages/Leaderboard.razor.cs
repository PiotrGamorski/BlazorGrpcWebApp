using BlazorGrpcWebApp.Client.Authentication;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;
using Grpc.Core;
using Microsoft.AspNetCore.Components;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Leaderboard : ComponentBase
    {
        private bool useGrcp;
        private int authUserId;
        public bool BattleCompleted { get; set; }
        private bool IsVictorious { get; set; }
        private string LeaderboardSearchString { get; set; } = string.Empty;
        public IList<GrpcUserGetLeaderboardResponse> UserLeaderboard { get; set; } = new List<GrpcUserGetLeaderboardResponse>();

        protected override async Task OnInitializedAsync()
        {
            useGrcp = bool.Parse(AppSettingsService.GetValueFromPagesSec("Leaderboard"));

            var authState = await ((CustomAuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
            authUserId = int.Parse(authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            BattleCompleted = false;

            if(useGrcp) await LeaderboardGrpcService.GetLeaderboardWithGrpc();
            else await LeaderboardRestService.GetLeaderboardWithRest();

            await PopulateUserLeaderboard();
        }

        private async Task PopulateUserLeaderboard()
        {
            if (useGrcp)
            {
                foreach (var item in LeaderboardGrpcService.Leaderboard)
                    UserLeaderboard.Add(item);
                foreach (var item in UserLeaderboard)
                    item.ShowLogs = await LeaderboardGrpcService.ShowBattleLogsWithGrpc(item.UserId);
            }
            else
            {
                foreach (var item in LeaderboardRestService.Leaderboard)
                    UserLeaderboard.Add(item);
                foreach (var item in UserLeaderboard)
                {
                    var response = await LeaderboardRestService
                        .ShowBattleLogsWithRest(new ShowBattleLogsRequest() { AuthUserId = authUserId, OpponentId = item.UserId });

                    if (response.Success)
                        item.ShowLogs = response.Data;
                    else item.ShowLogs = response.Success;
                }
            }
        }

        public async Task FightOpponent(int opponentId)
        {
            UserLeaderboard = new List<GrpcUserGetLeaderboardResponse>();
            BattleCompleted = false;
            try
            {
                if (useGrcp)
                {
                    IsVictorious = await BattleGrpcService.DoGrpcStartBattle(opponentId);
                    await LeaderboardGrpcService.GetLeaderboardWithGrpc();
                }
                else
                {
                    var response = await BattleRestService.StartBattle(new StartBattleRequest() { AuthUserId = authUserId, OpponentId = opponentId });
                    if (response.Success)
                    {
                        IsVictorious = response!.Data!.IsVictory;
                        await LeaderboardRestService.GetLeaderboardWithRest();
                    }
                    else
                    {
                        ToastService.ShowInfo(response.Message);
                        return;
                    }
                    
                }
                
                await PopulateUserLeaderboard();
                BattleCompleted = true;
                StateHasChanged();

                await BananaService.GrpcGetBananas();
                await BananaService.BananasChanged();
                if (BattleCompleted)
                {
                    if (IsVictorious)
                        ToastService.ShowSuccess("You've won!", "Success");
                    else ToastService.ShowError("You've lost", ":(");
                }
            }
            catch (RpcException e)
            {
                ToastService.ShowError(e.Status.ToString(), ":(");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message, "Error");
            }
        }

        public async Task<List<string>> GetBattleLogs(int opponentId)
        {
            if (bool.Parse(AppSettingsService.GetValueFromPagesSec("Leaderboard")))
                return await LeaderboardGrpcService.GetBattleLogsWithGrpc(opponentId);
            else
            {
                var response = await LeaderboardRestService
                    .GetBattleLogsWithRest(new GetBattleLogsRequest() { AuthUserId = authUserId, OpponentId = opponentId });
                var result = new List<string>();
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    foreach (var battleLog in response.Data)
                    { 
                        result.Add(battleLog.Log);
                    }
                    return result;
                }

                ToastService.ShowError(response.Message, "Error");
                return result;
            }     
        }
    }
}
