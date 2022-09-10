using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLastActivityController : ControllerBase
    {
        private readonly IUserLastActivityService _userLastActivityService;

        public UserLastActivityController(IUserLastActivityService userLastActivityService)
        {
            _userLastActivityService = userLastActivityService;
        }

        [HttpPost("getUserActivities")]
        public async Task<GenericAuthResponse<IList<UserLastActivityDto>>> GetUserLastActivities([FromBody]UserLastActivitiesRequestDto requestDto)
        {
            var result = await _userLastActivityService.GetUserLastActivites(requestDto.UserId, requestDto.Page, requestDto.ActivitiesNumber);

            if (result == null)
                return new GenericAuthResponse<IList<UserLastActivityDto>>() 
                { 
                    Success = false,
                    Data = result,
                    Message = "Not found"
                };
            return new GenericAuthResponse<IList<UserLastActivityDto>>()
            {
                Success = true,
                Data = result,
                Message = "Success"
            };
        }
    }
}
