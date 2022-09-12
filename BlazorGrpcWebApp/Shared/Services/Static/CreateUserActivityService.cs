using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Shared.Services.Static
{
    public static class CreateUserActivityService
    {
        private static async Task CreateActivity(DataContext dataContext, int userId, Activity activity, int? bananasSpent = null, int? bananasGained = null)
        {
            var userLastActivity = new UserLastActivity();
            userLastActivity.UserId = userId;
            userLastActivity.ExecutionDate = DateTime.Now;
            userLastActivity.LastActivityId = (await dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == activity))!.Id;
            userLastActivity.UserBananasTotal = (await dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId))!.Bananas;

            if (bananasSpent != null)
            {
                if (activity == Activity.BuildKnight || activity == Activity.BuildArcher || activity == Activity.BuildMage ||
                    activity == Activity.HealKnight || activity == Activity.HealArcher || activity == Activity.HealMage ||
                    activity == Activity.ReviveArmy)
                {
                    userLastActivity.UserBananasSpent = bananasSpent;
                }
            }

            if (bananasGained != null)
            {
                if (activity == Activity.DeleteKnight || activity == Activity.DeleteArcher || activity == Activity.DeleteMage)
                {
                    userLastActivity.UserBananasGained = bananasGained;
                }
            }

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

        public static async Task CreateReviveArmyActivity(DataContext dataContext, int userId, int? bananasSpent = 1000)
        {
            await CreateActivity(dataContext, userId, Activity.ReviveArmy, bananasSpent);
        }

        public static async Task CreateWinAFightActivity(DataContext dataContext, int userId)
        {
            await CreateActivity(dataContext, userId, Activity.WinAFight);
        }

        public static async Task CreateLostAFightActivity(DataContext dataContext, int userId)
        {
            await CreateActivity(dataContext, userId, Activity.LostAFight);
        }

        public static async Task CreateHealActivity(DataContext dataContext, int userId, string? unitTitle, int? bananasSpent = null)
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

            await CreateActivity(dataContext, userId, activity, bananasSpent, null);
        }

        public static async Task CreateBuildActivity(DataContext dataContext, int userId, string? unitTitle, int? bananasSpent = null)
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

            await CreateActivity(dataContext, userId, activity, bananasSpent);
        }

        public static async Task CreateDeleteActivity(DataContext dataContext, int userId, string? unitTitle, int? bananasGained = null)
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

            await CreateActivity(dataContext, userId, activity, null, bananasGained);
        }
    }
}
