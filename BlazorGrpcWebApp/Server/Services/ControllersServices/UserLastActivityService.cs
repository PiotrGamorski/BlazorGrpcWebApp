using AutoMapper;
using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Server.Services.ControllersServices
{
    public class UserLastActivityService : IUserLastActivityService
    {
        private DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserLastActivityService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<IList<UserLastActivityDto>?> GetUserLastActivites(int userId, Page page, int activitiesNumber)
        {
            if (page == Page.Army)
            {
                var userLastActivities = await _dataContext.UserLastActivities
                    .Where(a => a.UserId == userId)
                    .Where(a => a.LastActivity.ActivityType == Activity.HealKnight || a.LastActivity.ActivityType == Activity.HealArcher || a.LastActivity.ActivityType == Activity.HealMage
                           || a.LastActivity.ActivityType == Activity.BuildKnight || a.LastActivity.ActivityType == Activity.BuildArcher || a.LastActivity.ActivityType == Activity.BuildMage
                           || a.LastActivity.ActivityType == Activity.DeleteKnight || a.LastActivity.ActivityType == Activity.DeleteArcher || a.LastActivity.ActivityType == Activity.DeleteMage)
                    .Include(a => a.LastActivity)
                    .OrderByDescending(a => a.ExecutionDate)
                    .Take(activitiesNumber)
                    .ToListAsync();

                var result = new List<UserLastActivityDto>();
                foreach (var item in userLastActivities)
                { 
                    result.Add(_mapper.Map<UserLastActivityDto>(item));
                }

                return result;
            }

            return null;
        }
    }
}
