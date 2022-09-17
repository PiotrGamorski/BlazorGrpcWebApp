using BlazorGrpcWebApp.Shared.Dictionaries;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Enums;

namespace BlazorGrpcWebApp.Shared.Services.Static
{
    public static class UserActivityInfoService
    {
        public static string GetUserLastActivityInfo(UserLastActivityDto request)
        {
            switch (request.LastActivity)
            {
                case Activity.BuildKnight:
                    return string.Empty;
                case Activity.BuildArcher:
                    return string.Empty;
                case Activity.BuildMage:
                    return string.Empty;
                case Activity.DeleteKnight:
                    return ActivitiesDictionary.LastActivityDic[Activity.BuildKnight] + 
                        $". You gained {request.UserBananasGained} coins. Your total coins was {request.UserBananasTotal}.";
                case Activity.DeleteArcher:
                    return ActivitiesDictionary.LastActivityDic[Activity.DeleteArcher] +
                        $". You gained {request.UserBananasGained} coins. Your total coins was {request.UserBananasTotal}.";
                case Activity.DeleteMage:
                    return ActivitiesDictionary.LastActivityDic[Activity.DeleteMage] +
                        $". You gained {request.UserBananasGained} coins. Your total coins was {request.UserBananasTotal}.";
                case Activity.HealKnight:
                    return string.Empty;
                case Activity.HealArcher:
                    return string.Empty;
                case Activity.HealMage:
                    return string.Empty;
                case Activity.ReviveArmy:
                    return string.Empty;
                case Activity.WinAFight:
                    return string.Empty;
                case Activity.LostAFight:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
