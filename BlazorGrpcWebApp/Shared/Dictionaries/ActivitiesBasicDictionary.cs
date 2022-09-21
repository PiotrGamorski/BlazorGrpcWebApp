using BlazorGrpcWebApp.Shared.Enums;

namespace BlazorGrpcWebApp.Shared.Dictionaries
{
    public static class ActivitiesBasicDictionary
    {
        public static Dictionary<Activity, string> LastActivityDic = new Dictionary<Activity, string>()
        {
            { Activity.Register, ""},
            { Activity.Verify, ""},
            { Activity.VerificationCodeReset, ""},
            { Activity.PasswordReset, ""},
            { Activity.Login, "" },
            { Activity.BuildKnight, "Added a new Knight to your army"},
            { Activity.BuildArcher, "Added a new Archer to your army"},
            { Activity.BuildMage, "Added a new Mage to your army"},
            { Activity.HealKnight, "You healed a Knight"},
            { Activity.HealArcher, "Healed an Archer"},
            { Activity.HealMage, "Healed a Mage with"},
            { Activity.DeleteKnight, "Deleted a Knight from your army"},
            { Activity.DeleteArcher, "Deleted an Archer from your army"},
            { Activity.DeleteMage, "Deleted a Mage from your army"},
            { Activity.ReviveArmy, "Revied your army for 1000 coins"},
            { Activity.WinAFight, "Won a fight"},
            { Activity.LostAFight, "Lost a fight"},
        };
    }
}
