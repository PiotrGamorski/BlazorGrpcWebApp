using Grpc.Core;
using BlazorGrpcWebApp.Shared;

public class UnitServiceGrpcImpl : UnitServiceGrpc.UnitServiceGrpcBase
    {
    public override async Task GetUnits(UnitRequest request, IServerStreamWriter<UnitResponse> responseStream, ServerCallContext context)
    {
        IList<Unit> Units = new List<Unit>()
            {
                new Unit() { Id = 1, Title = "Knight", Attack = 10, Defense = 10, BananaCost = 100},
                new Unit() { Id = 2, Title = "Archer", Attack = 15, Defense = 5, BananaCost = 150},
                new Unit() { Id = 3, Title = "Mage", Attack = 20, Defense = 1, BananaCost = 200},
            };

        foreach (var unit in Units)
        {
            await responseStream.WriteAsync(new UnitResponse()
            {
                Id = unit.Id,
                Title = unit.Title,
                Attack = unit.Attack,
                Defense = unit.Defense,
                BananaCost = unit.BananaCost,
            });

        }
    }
}
