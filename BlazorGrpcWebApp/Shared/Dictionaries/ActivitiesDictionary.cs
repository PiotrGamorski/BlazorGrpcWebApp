using BlazorGrpcWebApp.Shared.Enums;

namespace BlazorGrpcWebApp.Shared.Dictionaries
{
    public static class ActivitiesDictionary
    {
        public static Dictionary<Activity, string> LastActivityText = new Dictionary<Activity, string>()
        {
            { Activity.Register, ""},
            { Activity.Verify, ""},
            { Activity.VerificationCodeReset, ""},
            { Activity.PasswordReset, ""},
            { Activity.Login, "" },
            { Activity.BuildKnight, "You added a new Knight to your army"},
            { Activity.BuildArcher, "You added a new Archer to your army"},
            { Activity.BuildMage, "You added a new Mage to your army"},
            { Activity.HealKnight, "You healed a Knight"},
            { Activity.HealArcher, "You healed an Archer"},
            { Activity.HealMage, "You healed a Mage with"},
            { Activity.DeleteKnight, "You deleted a Knight from your army"},
            { Activity.DeleteArcher, "You deleted an Archer from your army"},
            { Activity.DeleteMage, "You deleted a Mage from your army"},
            { Activity.ReviveArmy, "You revied your army for 1000 coins"},
            { Activity.WinAFight, "You won a fight"},
            { Activity.LostAFight, "You lost a fight"},
        };
    }
}
