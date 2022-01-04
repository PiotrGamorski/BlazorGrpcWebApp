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

    public override async Task GetGrpcUnits(GrpcUnit request, IServerStreamWriter<GrpcUnit> responseStream, ServerCallContext context)
    {
        var grpcUnits = _dataContext.GrpcUnits;
        if (grpcUnits != null)
        {
            foreach (var grpcUnit in grpcUnits)
            {
                await responseStream.WriteAsync(grpcUnit);
            }
        }
        else
            throw new RpcException(Status.DefaultCancelled);
    }

    public override async Task CreateGrpcUnit(GrpcUnitRequest request, IServerStreamWriter<GrpcUnitResponse> responseStream, ServerCallContext context)
    {
        await _dataContext.AddAsync(new GrpcUnit()
        {
            Title = request.GrpcUnit.Title,
            Attack = request.GrpcUnit.Attack,
            Defense = request.GrpcUnit.Defense,
            BananaCost = request.GrpcUnit.BananaCost,
            HitPoints = request.GrpcUnit.HitPoints,
        });
        await _dataContext.SaveChangesAsync();

        var grpcUnits = _dataContext.GrpcUnits;
        if (grpcUnits != null)
        {
            foreach (var grpcUnit in grpcUnits)
            {
                await responseStream.WriteAsync(new GrpcUnitResponse() { GrpcUnit = grpcUnit });
            }
        }
        else
            throw new RpcException(Status.DefaultCancelled);
    }

    public override async Task<GrpcUnitResponse> UpdateGrpcUnit(GrpcUnitRequest request, ServerCallContext context)
    {
        var dbGrpcUnit = _dataContext.GrpcUnits.FirstOrDefault(unit => unit.Id == request.GrpcUnit.Id);
        if (dbGrpcUnit != null)
        { 
            dbGrpcUnit.Title = request.GrpcUnit.Title;
            dbGrpcUnit.Attack = request.GrpcUnit.Attack;
            dbGrpcUnit.Defense = request.GrpcUnit.Defense;
            dbGrpcUnit.BananaCost = request.GrpcUnit.BananaCost;
            dbGrpcUnit.HitPoints = request.GrpcUnit.HitPoints;

            await _dataContext.SaveChangesAsync();
            return new GrpcUnitResponse() { GrpcUnit = dbGrpcUnit};
        }
        else throw new RpcException(Status.DefaultCancelled);
    }
}
