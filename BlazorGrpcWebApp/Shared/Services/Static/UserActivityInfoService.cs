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
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.BuildKnight] +
                        $". You spent {request.UserBananasSpent} coins and {request.UserBananasTotal} coins were left.";
                case Activity.BuildArcher:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.BuildArcher] +
                        $". You spent {request.UserBananasSpent} coins and {request.UserBananasTotal} coins were left.";
                case Activity.BuildMage:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.BuildMage] +
                        $". You spent {request.UserBananasSpent} coins and {request.UserBananasTotal} coins were left.";
                case Activity.DeleteKnight:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.DeleteKnight] + 
                        $". You gained {request.UserBananasGained} coins and {request.UserBananasTotal} coins were left.";
                case Activity.DeleteArcher:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.DeleteArcher] +
                        $". You gained {request.UserBananasGained} coins and {request.UserBananasTotal} coins were left.";
                case Activity.DeleteMage:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.DeleteMage] +
                        $". You gained {request.UserBananasGained} coins and {request.UserBananasTotal} coins were left.";
                case Activity.HealKnight:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.HealKnight] +
                        $". You spent {request.UserBananasSpent} coins and {request.UserBananasTotal} coins were left.";
                case Activity.HealArcher:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.HealArcher] +
                        $". You spent {request.UserBananasSpent} coins and {request.UserBananasTotal} coins were left.";
                case Activity.HealMage:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.HealMage] +
                        $". You spent {request.UserBananasSpent} coins and {request.UserBananasTotal} coins were left.";
                case Activity.ReviveArmy:
                    return ActivitiesBasicDictionary.LastActivityDic[Activity.ReviveArmy] +
                        $" and {request.UserBananasTotal} coins were left.";
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
