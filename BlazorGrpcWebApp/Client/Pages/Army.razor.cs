using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Helpers;
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
            await GetMyUnits();
        }

        private async Task GetMyUnits()
        {
            var UserUnitsResponses = (await ArmyService.RestApiGetUserUnits()).ToList();

            foreach (var item in UserUnitsResponses)
            {
                var temp = item.UnitId;
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
    }
}
