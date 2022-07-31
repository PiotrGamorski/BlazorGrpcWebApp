using BlazorGrpcWebApp.Client.Authentication;
using BlazorGrpcWebApp.Shared;
using Grpc.Core;
using Microsoft.AspNetCore.Components;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Leaderboard : ComponentBase
    {
        private int myUserId;
        public bool BattleCompleted { get; set; }
        private bool IsVictorious { get; set; }
        private string LeaderboardSearchString { get; set; } = string.Empty;
        public IList<GrpcUserGetLeaderboardResponse> MyLeaderboard { get; set; } = new List<GrpcUserGetLeaderboardResponse>();

        protected override async Task OnInitializedAsync()
        {
            var authState = await ((CustomAuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
            myUserId = int.Parse(authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            BattleCompleted = false;

            await LeaderboardService.DoGrpcGetLeaderboard();
            await PopulateMyLeaderBoard();
        }

        private async Task PopulateMyLeaderBoard()
        {
            foreach (var item in LeaderboardService.GrpcLeaderboardResponses)
            {
                MyLeaderboard.Add(item);
            }
            foreach (var item in MyLeaderboard)
            {
                item.ShowLogs = await LeaderboardService.DoGrpcShowBattleLogs(item.UserId);
            }
        }


        public async Task FightOpponentGrpc(int opponentId)
        {
            MyLeaderboard = new List<GrpcUserGetLeaderboardResponse>();
            BattleCompleted = false;
            try
            {
                IsVictorious = await GrpcBattleService.DoGrpcStartBattle(opponentId);
                await LeaderboardService.DoGrpcGetLeaderboard();
                await PopulateMyLeaderBoard();
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
            return await LeaderboardService.DoGrpcGetBattleLogs(opponentId);
        }
    }
}
