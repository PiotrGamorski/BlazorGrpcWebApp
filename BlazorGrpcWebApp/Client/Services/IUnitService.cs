using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Services
{
    public interface IUnitService
    {
        IList<Unit> Units { get; set; }
        IList<UserUnit> MyUnits { get; set;}
        Task AddUnit(int unitId);
        Task LoadUnitsAsync();
        Task<IList<GrpcUnit>> DoGetGrpcUnits(int deadline);
        Task<GrpcUnitResponse> DoUpdateGrpcUnit(GrpcUnitRequest grpcUnitRequest, int deadline);
    }
}
