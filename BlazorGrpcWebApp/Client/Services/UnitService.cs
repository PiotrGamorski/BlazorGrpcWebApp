using Blazored.Toast.Services;
using BlazorGrpcWebApp.Shared;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class UnitService : IUnitService
    {
        private readonly IToastService _toastService;
        private readonly HttpClient _httpClient;
        private readonly GrpcChannel _channel;
        private UnitServiceGrpc.UnitServiceGrpcClient _unitServiceGrpcClient;

        public UnitService(IToastService toastService, HttpClient httpClient)
        {
            _toastService = toastService;
            _httpClient = httpClient;
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _unitServiceGrpcClient = new UnitServiceGrpc.UnitServiceGrpcClient(_channel);
        }
        
        public IList<Unit> Units { get; set; } = new List<Unit>();
        public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();
        public IList<UnitResponse> UnitResponses { get; set; } = new List<UnitResponse>();

        public Task AddUnit(int unitId)
        {
            var unit = Units.First(u => u.Id == unitId);
            if (unit != null)
            {
                MyUnits.Add(new UserUnit { UnitId = unit.Id, HitPoints = unit.HitPoints });
                _toastService.ShowSuccess($"Your {unit.Title} has been built!", "Unit built!");
            }
            else
                throw new InvalidOperationException("Cannot add Unit which does not exist!");

            return Task.CompletedTask;
        }

        #region Rest API calls
        public async Task LoadUnitsAsync()
        {
            if (Units.Count == 0)
            {
                Units = await _httpClient.GetFromJsonAsync<IList<Unit>>("api/Unit");
            }
        }
        #endregion

        #region gRPC Calls
        public async Task<IList<UnitResponse>> DoGetUnits(int deadline)
        {
            var units = new List<UnitResponse>();
            try
            {
                var unitResponse = _unitServiceGrpcClient.GetUnits(new UnitRequest() { }, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
                while (await unitResponse.ResponseStream.MoveNext(new CancellationToken()))
                {
                    units.Add(unitResponse.ResponseStream.Current);
                }
                return await Task.FromResult(units);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                throw new RpcException(e.Status);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
            { 
                throw new RpcException(e.Status);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
    }
}
