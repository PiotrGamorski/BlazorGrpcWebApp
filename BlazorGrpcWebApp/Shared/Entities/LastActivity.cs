namespace BlazorGrpcWebApp.Shared.Entities
{
    public enum Activity 
    {
        Register,
        Verify,
        VerificationCodeReset,
        PasswordReset,
        Login,
        BuildKnight,
        BuildArcher,
        BuildMage,
        DeleteKnight,
        DeleteArcher,
        DeleteMage,
        HealKnight,
        HealArcher,
        HealMage,
        ReviveArmy,
        WinAFight,
        LostAFight
    }
    public class LastActivity
    {
        public int Id { get; set; }
        public Activity ActivityType { get; set; }
    }
}
