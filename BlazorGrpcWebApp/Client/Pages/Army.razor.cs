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
        private IList<MyUnit> MyUnits { get; set; } = new List<MyUnit>();

        public async Task PopulateMyArmy()
        {
            //await GetMyUnitsRestApi();
            await GetMyUnitsGrpc();
        }

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
                            img = ImgPath,
                            title = "Knight",
                            hitPoints = item.HitPoints
                        });
                        break;
                    case 2:
                        ImgPath = "/icons/archer.png";
                        MyUnits.Add(new MyUnit()
                        {
                            img = ImgPath,
                            title = "Archer",
                            hitPoints = item.HitPoints
                        });
                        break;
                    case 3:
                        ImgPath = "/icons/mage.png";
                        MyUnits.Add(new MyUnit()
                        {
                            img = ImgPath,
                            title = "Mage",
                            hitPoints = item.HitPoints
                        });
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task GetMyUnitsRestApi()
        {
            var UserUnitResponses = (await ArmyService.RestApiGetUserUnits()).ToList();
            PopulateMyUnits(UserUnitResponses);
        }

        private async Task GetMyUnitsGrpc()
        {
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
    }
}
