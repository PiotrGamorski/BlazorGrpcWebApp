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

    public override async Task GetGrpcUnits(GrpcUnitRequest request, IServerStreamWriter<GrpcUnitResponse> responseStream, ServerCallContext context)
    {
        var grpcUnits = _dataContext.GrpcUnits;
        if (grpcUnits != null && grpcUnits.Any())
        {
            foreach (var grpcUnit in grpcUnits)
            {
                await responseStream.WriteAsync(new GrpcUnitResponse() { GrpcUnit = grpcUnit });
            }
        }
        else
            throw new RpcException(new Status(StatusCode.NotFound, "No Units found."));
    }

    public override async Task<GrpcUnitResponse> CreateGrpcUnit(GrpcUnitRequest request, ServerCallContext context)
    {
        try
        {
            await _dataContext.AddAsync(request.GrpcUnit);
            await _dataContext.SaveChangesAsync();
            var createdGrpcUnit =_dataContext.GrpcUnits.FirstOrDefault(u => u.Id == request.GrpcUnit.Id);
            return new GrpcUnitResponse() { GrpcUnit = createdGrpcUnit };
        }
        catch(RpcException e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
    }

    public override async Task<GrpcUnitResponse> UpdateGrpcUnit(GrpcUnitRequest request, ServerCallContext context)
    {
        var dbGrpcUnit = _dataContext.GrpcUnits.FirstOrDefault(unit => unit.Id == request.GrpcUnit.Id);
        if (dbGrpcUnit != null)
        {
            try
            {
                // dbGrpcUnit = request.GrpcUnit;   - this might override Id
                dbGrpcUnit.Title = request.GrpcUnit.Title;
                dbGrpcUnit.Attack = request.GrpcUnit.Attack;
                dbGrpcUnit.Defense = request.GrpcUnit.Defense;
                dbGrpcUnit.BananaCost = request.GrpcUnit.BananaCost;
                dbGrpcUnit.HitPoints = request.GrpcUnit.HitPoints;

                await _dataContext.SaveChangesAsync();
                return new GrpcUnitResponse() { GrpcUnit = dbGrpcUnit };
            }
            catch (RpcException e)
            {
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }
        else throw new RpcException(new Status(StatusCode.NotFound, $"Unit with id {request.GrpcUnit.Id} not found."));
    }

    public override async Task<GrpcUnitDeleteResponse> DeleteGrpcUnit(GrpcUnitDeleteRequest request, ServerCallContext context)
    {
        var dbGrpcUnit = _dataContext.GrpcUnits.FirstOrDefault(u => u.Id == request.UnitId);
        if (dbGrpcUnit != null)
        {
            try
            {
                _dataContext.GrpcUnits.Remove(dbGrpcUnit);
                await _dataContext.SaveChangesAsync();
                return new GrpcUnitDeleteResponse() { UnitId = request.UnitId };
            }
            catch (RpcException e)
            {
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }
        else throw new RpcException(new Status(StatusCode.NotFound, $"Unith with id {request.UnitId} not found."));
    }
}
