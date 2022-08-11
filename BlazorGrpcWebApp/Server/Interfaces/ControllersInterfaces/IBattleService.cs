using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces
{
    public interface IBattleService
    {
        Task Fight(DataContext dataContext, User attacker, User opponent, BattleResult result);
    }
}
