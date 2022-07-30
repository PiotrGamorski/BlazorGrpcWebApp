using Grpc.Core;
using Microsoft.AspNetCore.Components;
using BlazorGrpcWebApp.Client.Dialogs;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using MudBlazor;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Army : ComponentBase
    {
        private string? ImgPath { get; set; }
        private IList<UserUnitDto>? UserUnitsDtos { get; set; }
        private IList<ArmyUnit>? ArmyUnits { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ArmyUnits = new List<ArmyUnit>();

            if (bool.Parse(AppSettingsService.GetValueFromPagesSec("Army")))
                UserUnitsDtos = await GetUserUnitsWithGrpc();
            else
                UserUnitsDtos = await GetUserUnitsWithRest();

            GetArmyUnits(UserUnitsDtos);
            StateHasChanged();
        }

        private Task ShowDeleteUserUnitDialog(int userUnitId)
        {
            var parameters = new DialogParameters();
            parameters.Add("Color", Color.Error);
            parameters.Add("Page", this);
            parameters.Add("UserUnitId", userUnitId);
            var options = new DialogOptions() { CloseButton = false };
            DialogService.Show<DeleteUserUnitDialog>("", parameters, options);
            return Task.CompletedTask;
        }

        private Task ShowReviveArmyDialog()
        {
            var parameters = new DialogParameters();
            parameters.Add("Color", Color.Error);
            parameters.Add("Page", this);
            var options = new DialogOptions() { CloseButton = false };
            DialogService.Show<ReviveArmyDialog>("", parameters, options);
            return Task.CompletedTask;
        }

        private void GetArmyUnits(IList<UserUnitDto>? UserUnitDtos)
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

        #region Rest
        private async Task<IList<UserUnitDto>?> GetUserUnitsWithRest()
        {
            IList<UserUnitDto>? userUnitsDtos;
            try
            {
                userUnitsDtos = (await ArmyRestService.GetArmy()).ToList();
                return userUnitsDtos;
            }
            catch (Exception e)
            {
                ToastService.ShowError($"{e.Message}", ":(");
            }

            return null;
        }

        private async Task ReviveArmy()
        {
            var result = await ArmyRestService.ReviveArmy();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                ToastService.ShowSuccess(await result.Content.ReadAsStringAsync());
            else
                ToastService.ShowError(await result.Content.ReadAsStringAsync());
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

        private async Task GetMyUnitsGrpc()
        {
            ArmyUnits = new List<ArmyUnit>();
            try
            {
                var UserUnitDtos = await GrpcUserUnitService.DoGrpcGetUserUnitAsync();
                GetArmyUnits(UserUnitDtos);
            }
            catch (RpcException e)
            {
                ToastService.ShowError($"{e.StatusCode}", ":(");
            }
            catch (Exception e)
            {
                ToastService.ShowError($"{e.Message}", ":(");
            }
        }

        private async Task HealUnitGrpc(int userUnitId)
        {
            var healUnitResponse = await ArmyGrpcService.DoGrpcHealUnit(userUnitId);
            await GetMyUnitsGrpc();
            await BananaService.GrpcGetBananas();
            await BananaService.BananasChanged();

            if (healUnitResponse.Success)
                ToastService.ShowSuccess(healUnitResponse.Message, "Success");
            else
                ToastService.ShowError(healUnitResponse.Message, ":(");
        }

        public async Task ReviveArmyGrpc()
        {
            var reviveArmyResponse = await ArmyGrpcService.DoGrpcReviveArmy();
            await GetMyUnitsGrpc();
            await BananaService.GrpcGetBananas();
            await BananaService.BananasChanged();

            if (reviveArmyResponse.Success)
                ToastService.ShowSuccess(reviveArmyResponse.Message, "Success");
            else
                ToastService.ShowError(reviveArmyResponse.Message, ":(");
        }

        public async Task DeleteUserUnitGrpc(int userUnitId)
        {
            var response = await GrpcUserUnitService.DoDeleteUserUnitGrpc(userUnitId);
            await GetMyUnitsGrpc();
            StateHasChanged();
            await BananaService.GrpcGetBananas();
            await BananaService.BananasChanged();
            if (response.Success)
                ToastService.ShowSuccess(response.Message, "Success");
            else ToastService.ShowError(response.Message, ":(");
        }
        #endregion
    }
}
