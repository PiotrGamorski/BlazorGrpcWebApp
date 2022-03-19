using BlazorGrpcWebApp.Client.Interfaces;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;
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

        public async Task<List<UserUnitResponse>> DoGrpcGetUserUnitAsync()
        {
            var result = new List<UserUnitResponse>();
            var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            try
            {
                var response = _userUnitServiceGrpcClient.GetUserUnits(new GrpcGetUserUnitRequest() { UserId = authUserId });
                while (await response.ResponseStream.MoveNext())
                {
                    result.Add(new UserUnitResponse()
                    {   
                        UserUnitId = response.ResponseStream.Current.UserunitId,
                        UnitId = response.ResponseStream.Current.UnitId,
                        HitPoints = response.ResponseStream.Current.HitPoints,
                    });
                }
                return await Task.FromResult(result);
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

        public async Task<DeleteGrpcUserUnitResponse> DoDeleteUserUnitGrpc(int userUnitId)
        {
            var authUserId = await _httpClient.GetFromJsonAsync<int>("api/user/getAuthUserId");
            var response = await _userUnitServiceGrpcClient.DeleteGrpcUserUnitAsync(new DeleteGrpcUserUnitRequest()
            {
                UserUnitId = userUnitId,
                AuthUserId = authUserId
            });

            return await Task.FromResult(response);
        }
    }
}
