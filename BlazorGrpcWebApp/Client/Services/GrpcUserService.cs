using BlazorGrpcWebApp.Shared;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace BlazorGrpcWebApp.Client.Services
{
    public class GrpcUserService : IGrpcUserService
    {
        private readonly GrpcChannel _channel;
        private UserServiceGrpc.UserServiceGrpcClient _userServiceGrpcClient;
        public GrpcUserService()
        {
            var httpClientGrpc = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            _channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions { HttpClient = httpClientGrpc });
            _userServiceGrpcClient = new UserServiceGrpc.UserServiceGrpcClient(_channel);
        }

        public async Task<RegisterGrpcUserResponse> Register(UserRegister request, int deadline)
        {
            return await DoGrpcUserRegister(new RegisterGrpcUserRequest()
            { 
                GrpcUser = new GrpcUser() 
                {
                   UserName = request.Username,
                   Email = request.Email,
                   Bananas = request.Bananas,
                   DateOfBirth = Timestamp.FromDateTime(new DateTime(request.DateOfBirth.Ticks, DateTimeKind.Utc)),
                   IsConfirmed = request.IsConfirmed,
                   DateCreated = Timestamp.FromDateTime(DateTime.UtcNow),
                },
                Password = request.Password,
            }, deadline);
        }

        #region Grpc Calls
        public async Task<RegisterGrpcUserResponse> DoGrpcUserRegister(RegisterGrpcUserRequest request, int deadline)
        {
            var grpcUser = new GrpcUserExistsRequest() { GrpcUser = request.GrpcUser };
            if ((await DoGrpcUserExists(grpcUser)).Exists)
                return new RegisterGrpcUserResponse() { Success = false, Message = "User already exists." };

            try
            {
                return await _userServiceGrpcClient.GrpcUserRegisterAsync(request, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                return new RegisterGrpcUserResponse() { Success = false, Message = e.Status.ToString() };
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal)
            {
                return new RegisterGrpcUserResponse() { Success = false, Message = e.Status.ToString() };
            }
            catch (Exception e)
            { 
                return new RegisterGrpcUserResponse() { Success = false, Message = e.Message };
            }
        }

        public async Task<LoginGrpcUserRespone> DoGrpcUserLogin(LoginGrpcUserRequest request, int deadline)
        {
            try
            {
                return await _userServiceGrpcClient.GrpcUserLoginAsync(request, deadline: DateTime.UtcNow.AddMilliseconds(deadline));
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                return new LoginGrpcUserRespone() { Success = false, Message = e.Status.ToString() };
            }
            catch (Exception e)
            {
                return new LoginGrpcUserRespone() { Success = false, Message = e.Message };
            }
        }

        public async Task<GrpcUserExistsResponse> DoGrpcUserExists(GrpcUserExistsRequest request)
        {
            return await _userServiceGrpcClient.GrpcUserExistsAsync(request);
        }
        #endregion
    }
}
