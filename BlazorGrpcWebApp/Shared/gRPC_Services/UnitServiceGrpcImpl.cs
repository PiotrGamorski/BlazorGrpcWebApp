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
                    HitPoints = unit.HitPoints,
                });
            }
        }
        else
            throw new RpcException(Status.DefaultCancelled);
        
    }

    public override async Task CreateUnit(UnitRequest request, IServerStreamWriter<UnitResponse> responseStream, ServerCallContext context)
    {
        await _dataContext.AddAsync(new Unit() 
        {
            Title = request.Title,
            Attack = request.Attack,
            Defense = request.Defense,
            BananaCost = request.BananaCost,
            HitPoints = request.HitPoints,
        });

        await _dataContext.SaveChangesAsync();
        //return await Task.FromResult(new UnitResponse() { });   // - if one wants to send created Unit instead of the whole list

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
                    HitPoints = unit.HitPoints,
                });
            }
        }
        else
            throw new RpcException(Status.DefaultCancelled);
    }
}
