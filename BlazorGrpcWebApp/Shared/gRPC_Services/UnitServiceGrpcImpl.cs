using Grpc.Core;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Data;

public class UnitServiceGrpcImpl : UnitServiceGrpc.UnitServiceGrpcBase
    {
    private readonly DataContext _dataContext;

    public UnitServiceGrpcImpl(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public override async Task GetUnits(UnitRequest request, IServerStreamWriter<UnitResponse> responseStream, ServerCallContext context)
    {
        var units = _dataContext.Units;

        if (units != null)
        {
            foreach (var unit in units)
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
}
