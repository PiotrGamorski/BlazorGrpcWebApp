using BlazorGrpcWebApp.Client.Authentication;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.Controllers_Models;
using Grpc.Core;
using Microsoft.AspNetCore.Components;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Leaderboard : ComponentBase
    {
        private int authUserId;
        public bool BattleCompleted { get; set; }
        private bool IsVictorious { get; set; }
        private string LeaderboardSearchString { get; set; } = string.Empty;
        public IList<GrpcUserGetLeaderboardResponse> UserLeaderboard { get; set; } = new List<GrpcUserGetLeaderboardResponse>();

        protected override async Task OnInitializedAsync()
        {
            var authState = await ((CustomAuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
            authUserId = int.Parse(authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            BattleCompleted = false;

            if(bool.Parse(AppSettingsService.GetValueFromPagesSec("Leaderboard")))
                await LeaderboardGrpcService.GetLeaderboardWithGrpc();
            else
                await LeaderboardRestService.GetLeaderboardWithRest();

            await PopulateAuthUserLeaderBoard();
        }

        private async Task PopulateAuthUserLeaderBoard()
        {
            foreach (var item in LeaderboardGrpcService.Leaderboard)
            {
                UserLeaderboard.Add(item);
            }
            foreach (var item in UserLeaderboard)
            {
                item.ShowLogs = await LeaderboardGrpcService.ShowBattleLogsWithGrpc(item.UserId);
            }
        }


        public async Task FightOpponentGrpc(int opponentId)
        {
            UserLeaderboard = new List<GrpcUserGetLeaderboardResponse>();
            BattleCompleted = false;
            try
            {
                IsVictorious = await GrpcBattleService.DoGrpcStartBattle(opponentId);
                await LeaderboardGrpcService.GetLeaderboardWithGrpc();
                await PopulateAuthUserLeaderBoard();
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
                ToastService.ShowError(e.Message, ":(");
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
