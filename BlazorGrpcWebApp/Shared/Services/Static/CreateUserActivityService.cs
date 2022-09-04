using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Shared.Services.Static
{
    public static class CreateUserActivityService
    {
        private static async Task CreateActivity(DataContext dataContext, int userId, Activity activity)
        {
            var userLastActivity = new UserLastActivitie();
            userLastActivity.UserId = userId;
            userLastActivity.ExecutionDate = DateTime.Now;
            userLastActivity.LastActivityId = (await dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == activity))!.Id;

            await dataContext.UserLastActivities.AddAsync(userLastActivity);
            await dataContext.SaveChangesAsync();
        }

        public static async Task CreateAuthActivity(DataContext dataContext, int userId, Activity activity)
        {
            if (activity == Activity.Login || activity == Activity.Register || activity == Activity.PasswordReset ||
                activity == Activity.Verify || activity == Activity.VerificationCodeReset)
            {
                await CreateActivity(dataContext, userId, activity);
            }

        }

        public static async Task CreateReviveArmyActivity(DataContext dataContext, int userId)
        {
            await CreateActivity(dataContext, userId, Activity.ReviveArmy);
        }

        public static async Task CreateWinAFightActivity(DataContext dataContext, int userId)
        {
            await CreateActivity(dataContext, userId, Activity.WinAFight);
        }

        public static async Task CreateLostAFightActivity(DataContext dataContext, int userId)
        {
            await CreateActivity(dataContext, userId, Activity.LostAFight);
        }

        public static async Task CreateHealActivity(DataContext dataContext, int userId, string? unitTitle)
        {
            var activity = new Activity();
            switch (unitTitle)
            {
                case "Knight":
                    activity = Activity.HealMage;
                    break;
                case "Archer":
                    activity = Activity.HealArcher;
                    break;
                case "Mage":
                    activity = Activity.HealMage;
                    break;
                default: break;
            }

            await CreateActivity(dataContext, userId, activity);
        }

        public static async Task CreateBuildActivity(DataContext dataContext, int userId, string? unitTitle)
        {
            var activity = new Activity();
            switch (unitTitle)
            {
                case "Knight":
                    activity = Activity.BuildKnight;
                    break;
                case "Archer":
                    activity = Activity.BuildArcher;
                    break;
                case "Mage":
                    activity = Activity.BuildMage;
                    break;
                default: break;
            }

            await CreateActivity(dataContext, userId, activity);
        }

        public static async Task CreateDeleteActivity(DataContext dataContext, int userId, string? unitTitle)
        {
            var activity = new Activity();
            switch (unitTitle)
            {
                case "Knight":
                    activity = Activity.DeleteKnight;
                    break;
                case "Archer":
                    activity = Activity.DeleteArcher;
                    break;
                case "Mage":
                    activity = Activity.DeleteMage;
                    break;
                default: break;
            }

            await CreateActivity(dataContext, userId, activity);
        }
    }
}
