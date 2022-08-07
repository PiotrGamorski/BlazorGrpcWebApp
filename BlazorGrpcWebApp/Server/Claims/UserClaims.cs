using BlazorGrpcWebApp.Shared.Entities;
using System.Security.Claims;

namespace BlazorGrpcWebApp.Server.Claims
{
    public static class UserClaims
    {
        private static List<Claim>? Claims;

        public static List<Claim> CreateClaims(User user)
        {
            var userRoles = user.Roles.Where(ur => ur.UserId == user.Id).ToList();

            Claims =  new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            foreach (var ur in userRoles)
            {
                Claims.Add(new Claim($"{ur.Role.Name}Role", ur.Role.Name));
            }

            return Claims;
        }
    }
}