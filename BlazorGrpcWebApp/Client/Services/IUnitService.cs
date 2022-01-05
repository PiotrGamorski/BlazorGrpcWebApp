using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Services
{
    public interface IUnitService
    {
        int deadline { get; set; }
        IList<Unit> Units { get; set; }
        IList<UserUnit> MyUnits { get; set;}
        Task AddUnit(int unitId);
        Task LoadUnitsAsync();
        Task<IList<GrpcUnitResponse>> DoGetGrpcUnits(int deadline);
        Task<GrpcUnitResponse> DoCreateGrpcUnit(GrpcUnitRequest grpcUnitRequest, int deadline);
        Task<GrpcUnitResponse> DoUpdateGrpcUnit(GrpcUnitRequest grpcUnitRequest, int deadline);
        Task<GrpcUnitDeleteResponse> DoDeleteGrpcUnit(GrpcUnitDeleteRequest grpcUnitDeleteRequest, int deadline);
    }
}
