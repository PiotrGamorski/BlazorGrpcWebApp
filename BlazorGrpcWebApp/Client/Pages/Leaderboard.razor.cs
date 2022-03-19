using BlazorGrpcWebApp.Client.Authentication;
using BlazorGrpcWebApp.Client.Dialogs;
using BlazorGrpcWebApp.Shared;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Leaderboard : ComponentBase
    {
        private int myUserId;
        public bool BattleCompleted { get; set; }
        private bool IsVictorious { get; set; }
        private string LeaderboardSearchString { get; set; } = string.Empty;
        public IList<GrpcUserGetLeaderboardResponse> MyLeaderboard = new List<GrpcUserGetLeaderboardResponse>();

        protected override async Task OnInitializedAsync()
        {
            var authState = await ((CustomAuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
            myUserId = int.Parse(authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            BattleCompleted = false;

            await LeaderboardService.DoGrpcGetLeaderboard();
            await PopulateMyLeaderBoard();
        }

        private string GetMyUserStyle(int userId)
        {
            if (userId == myUserId)
                return "color: crimson; font-weight: 600px;";
            else
                return string.Empty;
        }

        private Task PopulateMyLeaderBoard()
        {
            foreach (var item in LeaderboardService.GrpcLeaderboardResponses)
            {
                MyLeaderboard.Add(item);
            }
            return Task.CompletedTask;
        }

        private Task OpenFightBattleDialog(int opponentId)
        {
            var parameters = new DialogParameters();
            parameters.Add("Color", Color.Error);
            parameters.Add("Page", this);
            parameters.Add("OpponentId", opponentId);
            var options = new DialogOptions() { CloseButton = false };
            DialogService.Show<FightBattleDialog>("", parameters, options);
            return Task.CompletedTask;
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

        private bool Filter(GrpcUserGetLeaderboardResponse res) => FilterImplementation(res, LeaderboardSearchString);
        private bool FilterImplementation(GrpcUserGetLeaderboardResponse res, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return true;
            else if (res.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) return true;
            else return false;
        }
    }
}
