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

        public int deadline { get; set; } = 2000;
        public IList<Unit> Units { get; set; } = new List<Unit>();
        public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

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
#pragma warning disable CS8601 // Possible null reference assignment.
                Units = await _httpClient.GetFromJsonAsync<IList<Unit>>("api/Unit");
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }
        #endregion

        #region gRPC Calls
        public async Task<IList<GrpcUnitResponse>> DoGetGrpcUnits(int deadline)
        {
            var grpcUnitsResponses = new List<GrpcUnitResponse>();
            try
            {
                var response = _unitServiceGrpcClient.GetGrpcUnits(new GrpcUnitRequest() {}, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
                while (await response.ResponseStream.MoveNext(new CancellationToken()))
                {
                    grpcUnitsResponses.Add(response.ResponseStream.Current);
                }
                return await Task.FromResult(grpcUnitsResponses);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                throw new RpcException(e.Status);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
            {
                throw new RpcException(e.Status);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<GrpcUnitResponse> DoCreateGrpcUnit(GrpcUnitRequest grpcUnitRequest, int deadline)
        {
            try
            {
                return  await _unitServiceGrpcClient.CreateGrpcUnitAsync(grpcUnitRequest, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal)
            {
                throw new RpcException(e.Status);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<GrpcUnitResponse> DoUpdateGrpcUnit(GrpcUnitRequest grpcUnitRequest, int deadline)
        {
            try
            {
                return await _unitServiceGrpcClient.UpdateGrpcUnitAsync(grpcUnitRequest, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                throw new RpcException(e.Status);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
            {
                throw new RpcException(e.Status);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<GrpcUnitDeleteResponse> DoDeleteGrpcUnit(GrpcUnitDeleteRequest grpcUnitDeleteRequest, int deadline)
        {
            try
            {
                return await _unitServiceGrpcClient.DeleteGrpcUnitAsync(grpcUnitDeleteRequest, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
            {
                throw new RpcException(e.Status);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal)
            {
                throw new RpcException(e.Status);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
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
