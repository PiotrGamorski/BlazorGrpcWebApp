using AutoMapper;
using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Server.MappingProfile
{
    public class ServerAppMappingProfile : Profile
    {
        public ServerAppMappingProfile()
        {
            CreateMap<User, UserLeaderboardEntry>()
                .ForMember(m => m.UserId, c => c.MapFrom(u => u.Id))
                .ForMember(m => m.UserName, c => c.MapFrom(c => c.UserName))
                .ForMember(m => m.Victories, c => c.MapFrom(c => c.Victories))
                .ForMember(m => m.Defeats, c => c.MapFrom(c => c.Defeats))
                .ForMember(m => m.Battles, c => c.MapFrom(c => c.Battles));

            // This will work, when gRPC Services will be moved to Server Project
            CreateMap<User, GrpcUserGetLeaderboardResponse>()
                .ForMember(m => m.UserId, c => c.MapFrom(u => u.Id))
                .ForMember(m => m.UserName, c => c.MapFrom(c => c.UserName))
                .ForMember(m => m.Victories, c => c.MapFrom(c => c.Victories))
                .ForMember(m => m.Defeats, c => c.MapFrom(c => c.Defeats))
                .ForMember(m => m.Battles, c => c.MapFrom(c => c.Battles));
        }
    }
}
