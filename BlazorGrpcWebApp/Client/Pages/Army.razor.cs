using Grpc.Core;
using Microsoft.AspNetCore.Components;
using BlazorGrpcWebApp.Client.Dialogs;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using MudBlazor;
using BlazorGrpcWebApp.Client.Authentication;
using System.Security.Claims;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Army : ComponentBase
    {
        private bool useGrpc;
        private int authUserId;
        private string? ImgPath;
        private IList<UserUnitDto>? UserUnitsDtos { get; set; }
        private IList<ArmyUnit>? ArmyUnits { get; set; }


        protected override async Task OnInitializedAsync()
        {
            useGrpc = bool.Parse(AppSettingsService.GetValueFromPagesSec("Army"));

            var authState = await ((CustomAuthStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
            authUserId = int.Parse(authState.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            ArmyUnits = new List<ArmyUnit>();

            if (bool.Parse(AppSettingsService.GetValueFromPagesSec("Army")))
                UserUnitsDtos = await GetUserUnitsWithGrpc();
            else
                UserUnitsDtos = await GetUserUnitsWithRest();

            PopulateArmyUnits(UserUnitsDtos);
            StateHasChanged();
        }

        #region Page Data Loaders
        public void PopulateArmyUnits(IList<UserUnitDto>? UserUnitDtos)
        {
            foreach (var item in UserUnitDtos!)
            {
                switch (item.UnitId)
                {
                    case 1:
                        ImgPath = "/icons/knight.png";
                        ArmyUnits!.Add(new ArmyUnit()
                        {
                            UserUnitId = item.UserUnitId,
                            Img = ImgPath,
                            Title = "Knight",
                            HitPoints = item.HitPoints
                        });
                        break;
                    case 2:
                        ImgPath = "/icons/archer.png";
                        ArmyUnits!.Add(new ArmyUnit()
                        {
                            UserUnitId = item.UserUnitId,
                            Img = ImgPath,
                            Title = "Archer",
                            HitPoints = item.HitPoints
                        });
                        break;
                    case 3:
                        ImgPath = "/icons/mage.png";
                        ArmyUnits!.Add(new ArmyUnit()
                        {
                            UserUnitId = item.UserUnitId,
                            Img = ImgPath,
                            Title = "Mage",
                            HitPoints = item.HitPoints
                        });
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Dialogs
        public Task ShowDeleteUserUnitDialog(int userUnitId)
        {
            var parameters = new DialogParameters();
            parameters.Add("Color", Color.Error);
            parameters.Add("Page", this);
            parameters.Add("UserUnitId", userUnitId);
            var options = new DialogOptions() { CloseButton = false };
            DialogService.Show<DeleteUserUnitDialog>("", parameters, options);
            return Task.CompletedTask;
        }

        public Task ShowReviveArmyDialog()
        {
            var parameters = new DialogParameters();
            parameters.Add("Color", Color.Error);
            parameters.Add("Page", this);
            var options = new DialogOptions() { CloseButton = false };
            DialogService.Show<ReviveArmyDialog>("", parameters, options);
            return Task.CompletedTask;
        }
        #endregion

        #region Page Actions
        public async Task Heal(int userUnitId)
        {
            if (useGrpc) await HealUserUnitWithGrpc(userUnitId);
            else await HealUserUnitWithRest(userUnitId);
        }

        public async Task ReviveArmy()
        {
            if (useGrpc) await ReviveArmyWithGrpc();
            else await ReviveArmyWithRest();
        }

        public async Task Delete(int userUnitId)
        {
            if (useGrpc) await DeleteUserUnitGrpc(userUnitId);
            else await DeleteUserUnitWithRest(userUnitId);
        }
        #endregion

        #region Rest
        private async Task<IList<UserUnitDto>?> GetUserUnitsWithRest()
        {
            IList<UserUnitDto>? userUnitsDtos;
            try
            {
                userUnitsDtos = (await ArmyRestService.GetUserUnits()).ToList();
                return userUnitsDtos;
            }
            catch (Exception e)
            {
                ToastService.ShowError($"{e.Message}", ":(");
            }

            return null;
        }

        private async Task DeleteUserUnitWithRest(int userUnitId)
        {
            try
            {
                var response = await ArmyRestService.DeleteUserUnit(userUnitId);

                if (response.IsSuccessStatusCode)
                {
                    var userUnitsDtos = await GetUserUnitsWithRest();
                    PopulateArmyUnits(UserUnitsDtos);
                    StateHasChanged();

                    await BananaRestService.GetBananas(authUserId);
                    await BananaRestService.BananasChanged();

                    ToastService.ShowSuccess("Your Unit has been deleted", "Success");
                }
                else
                    ToastService.ShowError("Something went wrong...", ":(");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message);
            }
        }

        private async Task HealUserUnitWithRest(int userUnitId)
        {
            try
            {
                var response = await ArmyRestService.HealUserUnit(userUnitId);

                if (response.IsSuccessStatusCode)
                {
                    var userUnitsDtos = await GetUserUnitsWithRest();
                    PopulateArmyUnits(UserUnitsDtos);
                    StateHasChanged();

                    await BananaRestService.GetBananas(authUserId);
                    await BananaRestService.BananasChanged();

                    ToastService.ShowSuccess(response.Content.ToString());
                }
                else ToastService.ShowError(response.Content.ToString(), ":(");
            }
            catch (Exception e)
            {
                ToastService.ShowError(e.Message, ":(");
            }
        }

        private async Task ReviveArmyWithRest()
        {
            var result = await ArmyRestService.ReviveUserUnits();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userUnitsDtos = await GetUserUnitsWithRest();
                PopulateArmyUnits(UserUnitsDtos);
                StateHasChanged();

                await BananaRestService.GetBananas(authUserId);
                await BananaRestService.BananasChanged();

                ToastService.ShowSuccess(await result.Content.ReadAsStringAsync());
            }
            else ToastService.ShowError(await result.Content.ReadAsStringAsync());
        }
        #endregion

        #region gRPC
        private async Task<IList<UserUnitDto>?> GetUserUnitsWithGrpc()
        {
            IList<UserUnitDto> userUnitsDtos;
            try
            {
                userUnitsDtos = await GrpcUserUnitService.DoGrpcGetUserUnitAsync();
                return userUnitsDtos;
            }
            catch (RpcException e)
            {
                ToastService.ShowError($"{e.StatusCode}", ":(");
            }
            catch (Exception e)
            {
                ToastService.ShowError($"{e.Message}", ":(");
            }

            return null;
        }

        private async Task DeleteUserUnitGrpc(int userUnitId)
        {
            var response = await GrpcUserUnitService.DoDeleteUserUnitGrpc(userUnitId);

            if (response.Success)
            {
                ArmyUnits = new List<ArmyUnit>();
                var userUnitsDtos = await GetUserUnitsWithGrpc();
                PopulateArmyUnits(userUnitsDtos);
                StateHasChanged();

                await BananaService.GrpcGetBananas();
                await BananaService.BananasChanged();

                ToastService.ShowSuccess(response.Message, "Success");
            }
            else ToastService.ShowError(response.Message, "Error");
        }

        private async Task HealUserUnitWithGrpc(int userUnitId)
        {
            var response = await ArmyGrpcService.DoGrpcHealUnit(userUnitId);

            if (response.Success)
            {
                var userUnitsDtos = await GetUserUnitsWithGrpc();
                PopulateArmyUnits(userUnitsDtos);
                StateHasChanged();

                await BananaService.GrpcGetBananas();
                await BananaService.BananasChanged();

                ToastService.ShowSuccess(response.Message, "Success");
            }
            else
                ToastService.ShowError(response.Message, ":(");
        }

        private async Task ReviveArmyWithGrpc()
        {
            var reviveArmyResponse = await ArmyGrpcService.DoGrpcReviveArmy();

            if (reviveArmyResponse.Success)
            {
                var userUnitsDtos = await GetUserUnitsWithGrpc();
                PopulateArmyUnits(userUnitsDtos);
                StateHasChanged();

                await BananaService.GrpcGetBananas();
                await BananaService.BananasChanged();

                ToastService.ShowSuccess(reviveArmyResponse.Message, "Success");
            }
            else
                ToastService.ShowError(reviveArmyResponse.Message, ":(");
        }
        #endregion
    }
}
