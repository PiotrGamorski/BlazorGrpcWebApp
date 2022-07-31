using AutoMapper;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.MappingProfile
{
    public class ClientAppMappingProfile : Profile
    {
        public ClientAppMappingProfile()
        {
            CreateMap<UserLeaderboardEntry, GrpcUserGetLeaderboardResponse>();
        }
    }
}
