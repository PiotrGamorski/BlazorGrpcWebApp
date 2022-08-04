using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Entities;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IUnitService
    {
        int deadline { get; set; }
        IList<Unit> Units { get; set; }
        IList<UserUnit> UserUnits { get; set;}
        Task AddUnit(int unitId, int authUserId);
        Task LoadUnitsAsync();
        Task<IList<GrpcUnitResponse>> DoGetGrpcUnits(int deadline);
        Task<GrpcUnitResponse> DoCreateGrpcUnit(GrpcUnitRequest grpcUnitRequest, int deadline);
        Task<GrpcUnitResponse> DoUpdateGrpcUnit(GrpcUnitRequest grpcUnitRequest, int deadline);
        Task<GrpcUnitDeleteResponse> DoDeleteGrpcUnit(GrpcUnitDeleteRequest grpcUnitDeleteRequest, int deadline);
    }
}
