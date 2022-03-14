using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Entities;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System.Net.Http.Json;

namespace BlazorGrpcWebApp.Client.Services
{
    public class GrpcUserUnitService : IGrpcUserUnitService
    {
        private readonly GrpcChannel _channel;
        private UserUnitServiceGrpc.UserUnitServiceGrpcClient _userUnitServiceGrpcClient;
        private readonly HttpClient _httpClient;
        public GrpcUserUnitService(HttpClient httpClient)
        {
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _userUnitServiceGrpcClient = new UserUnitServiceGrpc.UserUnitServiceGrpcClient(_channel);
            _httpClient = httpClient;
        }

        public async Task<GrpcUserUnitResponse> DoGrpcBuildUserUnit(int unitId)
        {
            var user = await _httpClient.GetFromJsonAsync<User>("api/user/getAuthUser");
            try
            {
                var request = new GrpcUserUnitRequest()
                {
                    UnitId = unitId,
                    UserId = user!.Id,
                    Bananas = user!.Bananas,
                };
                return await _userUnitServiceGrpcClient.BuildUserUnitAsync(request);
            }
            catch(RpcException e) when (e.StatusCode == StatusCode.Unavailable)
            {
                return new GrpcUserUnitResponse() { Success = false, Message = e.Status.ToString() };
            }
        }
    }
}
