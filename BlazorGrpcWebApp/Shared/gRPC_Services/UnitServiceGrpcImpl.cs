using Grpc.Core;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;

public class UnitServiceGrpcImpl : UnitServiceGrpc.UnitServiceGrpcBase
    {
    private readonly DataContext _dataContext;

    public UnitServiceGrpcImpl(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public override async Task GetGrpcUnits(GrpcUnitRequest request, IServerStreamWriter<GrpcUnitResponse> responseStream, ServerCallContext context)
    {
        var units = _dataContext.Units;
        if (units != null && units.Any())
        {
            foreach (var unit in units)
            {
                await responseStream.WriteAsync(new GrpcUnitResponse() 
                { 
                    GrpcUnit = new GrpcUnit()
                    { 
                        Id = unit.Id,
                        Attack = unit.Attack,
                        Defense = unit.Defense,
                        HitPoints = unit.HitPoints,
                        BananaCost = unit.BananaCost,
                        Title = unit.Title,
                    }
                });
            }
        }
        else throw new RpcException(new Status(StatusCode.NotFound, "No Units found."));
    }

    public override async Task<GrpcUnitResponse> CreateGrpcUnit(GrpcUnitRequest request, ServerCallContext context)
    {
        try
        {
            await _dataContext.Units.AddAsync(new Unit()
            { 
                Id = request.GrpcUnit.Id,
                Attack = request.GrpcUnit.Attack,
                Defense = request.GrpcUnit.Defense,
                HitPoints = request.GrpcUnit.HitPoints,
                BananaCost = request.GrpcUnit.BananaCost,
                Title = request.GrpcUnit.Title,
            });
            await _dataContext.SaveChangesAsync();
            var unit =_dataContext.Units.FirstOrDefault(u => u.Id == request.GrpcUnit.Id);
            return new GrpcUnitResponse() 
            { 
                GrpcUnit = new GrpcUnit() 
                {
                    Id = unit!.Id,
                    Title = unit.Title,
                    Attack = unit.Attack,
                    Defense = unit.Defense,
                    HitPoints = unit.HitPoints,
                    BananaCost = unit.BananaCost,
                }
            };
        }
        catch(RpcException e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
    }

    public override async Task<GrpcUnitResponse> UpdateGrpcUnit(GrpcUnitRequest request, ServerCallContext context)
    {
        var unit = _dataContext.Units.FirstOrDefault(unit => unit.Id == request.GrpcUnit.Id);
        if (unit != null)
        {
            try
            {
                // dbGrpcUnit = request.GrpcUnit;   - this might override Id
                unit.Title = request.GrpcUnit.Title;
                unit.Attack = request.GrpcUnit.Attack;
                unit.Defense = request.GrpcUnit.Defense;
                unit.BananaCost = request.GrpcUnit.BananaCost;
                unit.HitPoints = request.GrpcUnit.HitPoints;

                await _dataContext.SaveChangesAsync();
                return new GrpcUnitResponse() 
                { 
                    GrpcUnit = new GrpcUnit()
                    { 
                        Id = request.GrpcUnit.Id,
                        Title = unit.Title,
                        Attack = unit.Attack,
                        Defense = unit.Defense,
                        BananaCost = unit.BananaCost,
                        HitPoints = unit.HitPoints,
                    }
                };
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
        var unit = _dataContext.Units.FirstOrDefault(u => u.Id == request.UnitId);
        if (unit != null)
        {
            try
            {
                _dataContext.Units.Remove(unit);
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
