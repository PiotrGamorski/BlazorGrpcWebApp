using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Helpers;
using Grpc.Core;
using Microsoft.AspNetCore.Components;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Army : ComponentBase
    {
        private string? ImgPath { get; set; }
        private IList<GrpcUnitResponse> grpcUnitsResponses { get; set; } = new List<GrpcUnitResponse>();
        private IList<MyUnit> MyUnits { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //await UnitService.LoadUnitsAsync();
            grpcUnitsResponses = await UnitService.DoGetGrpcUnits(UnitService.deadline);

            //await GetMyUnitsRestApi();
            await GetMyUnitsGrpc();
            StateHasChanged();
        }

        #region Simplyfying Methods
        private void PopulateMyUnits(List<UserUnitResponse> UserUnitResponses)
        {
            foreach (var item in UserUnitResponses)
            {
                switch (item.UnitId)
                {
                    case 1:
                        ImgPath = "/icons/knight.png";
                        MyUnits.Add(new MyUnit()
                        {
                            UserUnitId = item.UserUnitId,
                            Img = ImgPath,
                            Title = "Knight",
                            HitPoints = item.HitPoints
                        });
                        break;
                    case 2:
                        ImgPath = "/icons/archer.png";
                        MyUnits.Add(new MyUnit()
                        {
                            UserUnitId = item.UserUnitId,
                            Img = ImgPath,
                            Title = "Archer",
                            HitPoints = item.HitPoints
                        });
                        break;
                    case 3:
                        ImgPath = "/icons/mage.png";
                        MyUnits.Add(new MyUnit()
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

        #region REST Api Calls
        private async Task GetMyUnits()
        {
            var UserUnitResponses = (await ArmyService.RestApiGetUserUnits()).ToList();
            PopulateMyUnits(UserUnitResponses);
        }

        private async Task ReviveArmy()
        {
            var result = await ArmyService.RestApiReviveArmy();
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                ToastService.ShowSuccess(await result.Content.ReadAsStringAsync());
            else
                ToastService.ShowError(await result.Content.ReadAsStringAsync());
        }
        #endregion

        private async Task GetMyUnitsGrpc()
        {
            MyUnits = new List<MyUnit>();
            try
            {
                var UserUnitResponses = await GrpcUserUnitService.DoGrpcGetUserUnitAsync();
                PopulateMyUnits(UserUnitResponses);
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
            var healUnitResponse = await ArmyService.DoGrpcHealUnit(userUnitId);
            await GetMyUnitsGrpc();
            await BananaService.GrpcGetBananas();
            await BananaService.BananasChanged();

            if (healUnitResponse.Success)
                ToastService.ShowSuccess(healUnitResponse.Message, "Success");
            else
                ToastService.ShowError(healUnitResponse.Message, ":(");
        }

        private async Task ReviveArmyGrpc()
        {
            var reviveArmyResponse = await ArmyService.DoGrpcReviveArmy();
            await GetMyUnitsGrpc();
            await BananaService.GrpcGetBananas();
            await BananaService.BananasChanged();

            if (reviveArmyResponse.Success)
                ToastService.ShowSuccess(reviveArmyResponse.Message, "Success");
            else
                ToastService.ShowError(reviveArmyResponse.Message, ":(");

        }
    }
}
