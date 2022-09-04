using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorGrpcWebApp.Shared.Enums
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
}
