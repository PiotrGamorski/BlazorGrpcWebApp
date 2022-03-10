using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorGrpcWebApp.Server.Services
{
    public class UtilityService : IUtilityService
    {
        // IHttpContextAccessor solves the problem of User (Claims principals) problem faced in grpc services
        // as this class does not inherit from ComponentBase in comparison to Controllers
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;
        
        public UtilityService(IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier));
        public async Task<User?> GetUser() => await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == GetUserUserId());
    }
}
