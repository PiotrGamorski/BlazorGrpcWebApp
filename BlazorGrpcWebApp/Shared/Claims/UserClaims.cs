using BlazorGrpcWebApp.Shared.Entities;
using System.Security.Claims;

namespace BlazorGrpcWebApp.Shared.Claims
{
    public static class UserClaims
    {
        private static List<Claim>? Claims;

        public static List<Claim> CreateClaims(User user, List<UserRole> userRoles)
        {
            Claims =  new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            if (userRoles.Any())
            {
                foreach (var userRole in userRoles)
                {
                    Claims.Add(new Claim($"{userRole.Role.Name}Role", userRole.Role.Name));
                }
            }

            return Claims;
        }
    }
}