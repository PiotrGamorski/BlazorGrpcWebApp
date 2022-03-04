using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.MudTablesModels;
using Microsoft.AspNetCore.Components;

namespace BlazorGrpcWebApp.Client.Pages
{
    public partial class Army : ComponentBase
    {
        private string? ImgPath { get; set; }
        private IList<GrpcUnitResponse> grpcUnitsResponses { get; set; } = new List<GrpcUnitResponse>();
        private IList<MyUnit> MyUnitsToMudTable { get; set; } = new List<MyUnit>();


        public Task PopulateMyUnitsToMudTable()
        {
            foreach (var myUnit in GetMyUnits())
            {
                MyUnitsToMudTable.Add(myUnit);
                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        private IEnumerable<MyUnit> GetMyUnits()
        {
            foreach (var myUnit in UnitService.MyUnits)
            {
                switch (myUnit.UnitId)
                {
                    case 1:
                        ImgPath = "/icons/knight.png";
                        yield return new MyUnit()
                        {
                            img = ImgPath,
                            title = grpcUnitsResponses.FirstOrDefault(r => r.GrpcUnit.Id == myUnit.UnitId)!.GrpcUnit.Title,
                            userUnit = myUnit
                        };
                        break;
                    case 2:
                        ImgPath = "/icons/archer.png";
                        yield return new MyUnit()
                        {
                            img = ImgPath,
                            title = grpcUnitsResponses.FirstOrDefault(r => r.GrpcUnit.Id == myUnit.UnitId)!.GrpcUnit.Title,
                            userUnit = myUnit
                        };
                        break;
                    case 3:
                        ImgPath = "/icons/mage.png";
                        yield return new MyUnit()
                        {
                            img = ImgPath,
                            title = grpcUnitsResponses.FirstOrDefault(r => r.GrpcUnit.Id == myUnit.UnitId)!.GrpcUnit.Title,
                            userUnit = myUnit
                        };
                        break;
                }
            }
        }
    }
}
