using Blazored.Toast.Services;
using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Entities;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class UnitService : IUnitService
    {
        private readonly IToastService _toastService;
        private readonly IBananaService _bananaService;
        private readonly HttpClient _httpClient;
        private readonly GrpcChannel _channel;
        private UnitServiceGrpc.UnitServiceGrpcClient _unitServiceGrpcClient;

        public UnitService(IToastService toastService, IBananaService bananaService, HttpClient httpClient)
        {
            _toastService = toastService;
            _bananaService = bananaService;
            _httpClient = httpClient;
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _unitServiceGrpcClient = new UnitServiceGrpc.UnitServiceGrpcClient(_channel);
        }

        public int deadline { get; set; } = 3000;
        public IList<Unit> Units { get; set; } = new List<Unit>();
        public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

        #region Rest API calls
        public async Task LoadUnitsAsync()
        {
            if (Units.Count == 0)
            {
                Units = (await _httpClient.GetFromJsonAsync<IList<Unit>>("api/Unit"))!;
            }
        }

        public async Task AddUnit(int unitId)
        {
            var unit = Units.First(u => u.Id == unitId);
            if (unit != null)
            {
                var result = await _httpClient.PostAsJsonAsync<int>("api/userunit", unitId);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    _toastService.ShowError(await result.Content.ReadAsStringAsync());
                else
                {
                    await _bananaService.GetBananas();
                    MyUnits.Add(new UserUnit { UnitId = unit.Id, HitPoints = unit.HitPoints });
                    _toastService.ShowSuccess($"Your {unit.Title} has been built!", "Unit built!");
                }
            }
            else throw new InvalidOperationException("Cannot add Unit which does not exist!");
        }
        #endregion

        #region gRPC Calls
        public async Task<IList<GrpcUnitResponse>> DoGetGrpcUnits(int deadline)
        {
            var Units = new List<GrpcUnitResponse>();
            try
            {
                var response = _unitServiceGrpcClient.GetGrpcUnits(new GrpcUnitRequest() {}, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
                while (await response.ResponseStream.MoveNext(new CancellationToken()))
                {
                    Units.Add(response.ResponseStream.Current);
                }
                return await Task.FromResult(Units);
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
