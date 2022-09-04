using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Shared.Services
{
    public static class DeleteActivityService
    {
        private static async Task DeleteOldestActivity(DataContext dataContext, int userId, List<int> activitiesIds)
        {
            if (activitiesIds != null && activitiesIds.Count() == 3)
            {
                var userAcivitiesOfCommonType = await dataContext.UserLastActivities
                .Where(a => a.UserId == userId && (a.LastActivityId == activitiesIds[0] || a.LastActivityId == activitiesIds[1] || a.LastActivityId == activitiesIds[2]))
                .ToListAsync();

                if (userAcivitiesOfCommonType != null && userAcivitiesOfCommonType.Count() > 10)
                {
                    var userLastActivity = userAcivitiesOfCommonType.OrderBy(a => a.ExecutionDate).First();
                    dataContext.UserLastActivities.Remove(userLastActivity);
                    await dataContext.SaveChangesAsync();
                }
            }
        }
        public static async Task DeleteOldestActivity(DataContext dataContext, int userId, ActivitySimplified lastActivity)
        {
            if (lastActivity == ActivitySimplified.Heal)
            {
                var healActivitiesIds = await dataContext.LastActivities
                    .Where(a => a.ActivityType == Activity.HealKnight || a.ActivityType == Activity.HealArcher || a.ActivityType == Activity.HealMage)
                    .Select(a => a.Id)
                    .ToListAsync();

                await DeleteOldestActivity(dataContext, userId, healActivitiesIds);
            }

            if (lastActivity == ActivitySimplified.Build)
            {
                var buildActivitiesIds = await dataContext.LastActivities
                    .Where(a => a.ActivityType == Activity.BuildKnight || a.ActivityType == Activity.BuildArcher || a.ActivityType == Activity.BuildMage)
                    .Select(a => a.Id)
                    .ToListAsync();

                await DeleteOldestActivity(dataContext, userId, buildActivitiesIds);
            }

            if (lastActivity == ActivitySimplified.Delete)
            {
                var deleteActivitiesIds = await dataContext.LastActivities
                    .Where(a => a.ActivityType == Activity.DeleteKnight || a.ActivityType == Activity.DeleteArcher || a.ActivityType == Activity.DeleteMage)
                    .Select(a => a.Id)
                    .ToListAsync();

                await DeleteOldestActivity(dataContext, userId, deleteActivitiesIds);
            }
        }
        public static async Task DeleteOldestActivity(DataContext dataContext, int userId, Activity lastActivity)
        {
            var lastActivityId = (await dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == lastActivity))!.Id;
            var userLastActivities = await dataContext.UserLastActivities
                .Where(a => a.UserId == userId && a.LastActivityId == lastActivityId)
                .ToListAsync();

            if (userLastActivities != null && userLastActivities.Count() > 10)
            {
                var oldestActivity = userLastActivities.OrderBy(a => a.ExecutionDate).First();
                dataContext.UserLastActivities.Remove(oldestActivity);
                await dataContext.SaveChangesAsync();
            }
        }
    }
}
