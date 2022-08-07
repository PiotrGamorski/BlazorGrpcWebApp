using BlazorGrpcWebApp.Shared.Entities;
using System.Security.Claims;

namespace BlazorGrpcWebApp.Server.Claims
{
    public static class UserClaims
    {
        private static List<Claim> Claims;
        //private static List<UserRole> Roles;

        public static List<Claim> CreateClaims(User user)
        {
            //Roles = user.Roles;
            //var temp = Roles.FirstOrDefault(ur => ur.RoleId == 1);
            Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            return Claims;
        }
    }
}