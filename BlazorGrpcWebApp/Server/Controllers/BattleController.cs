using BlazorGrpcWebApp.Server.Interfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BattleController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUtilityService _utilityService;
        public BattleController(DataContext dataContext, IUtilityService utilityService)
        {
            _dataContext = dataContext;
            _utilityService = utilityService;
        }

        [HttpPost("reviveArmy")]
        public async Task<IActionResult> ReviveArmy()
        {
            var user = await _utilityService.GetUser();
            var userUnits = await _dataContext.UserUnits
                .Where(u => u.UserId == user!.Id)
                .Include(u => u.Unit)
                .ToArrayAsync();

            int bananasCost = 1000;
            if (user!.Bananas < bananasCost)
                return BadRequest($"Not enough bananas! You need {bananasCost} to revive your army.");

            bool armyAlreadyAlive = true;
            foreach (var userUnit in userUnits)
            {
                if (userUnit.HitPoints <= 0)
                { 
                    armyAlreadyAlive = false;
                    userUnit.HitPoints = new Random().Next(0, userUnit.Unit.HitPoints);
                }
            }

            if (armyAlreadyAlive)
                return Ok("Your army is already alive.");

            user.Bananas -= bananasCost;
            await _dataContext.SaveChangesAsync();
            return Ok("Army revived!");
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle([FromBody] int opponentId)
        {
            var attacker = await _utilityService.GetUser();
            var opponent = await _dataContext.Users.FindAsync(opponentId);
            if (opponent == null || opponent.IsDeleted)
                return NotFound("Opponent not available");

            var result = new BattleResult();
            await Fight(attacker!, opponent, result);
            
            return Ok(result);
        }

        private async Task Fight(User attacker, User opponent, BattleResult result)
        {
            var attackerArmy = await _dataContext.UserUnits
                .Where(u => u.UserId == attacker.Id && u.HitPoints > 0)
                .Include(u => u.Unit) 
                .ToListAsync();
            // one can include Unit as it appears in UserUnit class
            var opponentArmy = await _dataContext.UserUnits
                .Where(u => u.UserId == opponent.Id && u.HitPoints > 0)
                .Include(u => u.Unit)
                .ToListAsync();

            var attackerDamageSum = 0;
            var opponentDamageSum = 0;
            var currentRound = 0;
            while (attackerArmy.Count() > 0 && opponentArmy.Count() > 0)
            { 
                currentRound++;
                if (currentRound % 2 != 0)
                    attackerDamageSum += await FightRound(attacker, opponent, attackerArmy, opponentArmy, result);
                else
                    opponentDamageSum += await FightRound(opponent, attacker, opponentArmy, attackerArmy, result);
            }

            result.IsVictory = opponentArmy.Count() == 0;
            result.RoundsFought = currentRound;

            if (result.RoundsFought > 0)
                await FinishFight(attacker, opponent, result, attackerDamageSum, opponentDamageSum);
        }

        private Task<int> FightRound(User attacker, User opponent,
            List<UserUnit> attackerArmy, List<UserUnit> opponentArmy, BattleResult result)
        {
            int randomAttackerIndex = new Random().Next(attackerArmy.Count());
            int randomOpponentIndex = new Random().Next(opponentArmy.Count());

            var randomAttacker = attackerArmy[randomAttackerIndex];
            var randomOpponent = opponentArmy[randomOpponentIndex];

            var damage = new Random().Next(randomAttacker.Unit.Attack) - new Random().Next(randomOpponent.Unit.Defense);

            if (damage < 0) damage = 0;
            if (damage <= randomOpponent.HitPoints)
            {
                randomOpponent.HitPoints -= damage;
                result.Log.Add($"{attacker.UserName}'s {randomAttacker.Unit.Title} attacks " +
                               $"{opponent.UserName}'s {randomOpponent.Unit.Title} with {damage} damage.");
                return Task.FromResult(damage);
            }
            else
            {
                damage = randomOpponent.HitPoints;
                randomOpponent.HitPoints = 0;
                opponentArmy.Remove(randomOpponent);
                result.Log.Add($"{attacker.UserName}'s {randomAttacker.Unit.Title} kills " +
                               $"{opponent.UserName}'s {randomOpponent.Unit.Title} with {damage} damage.");
                return Task.FromResult(damage);
            }
        }

        private async Task FinishFight(User attacker, User opponent, BattleResult result,
            int attackerDamageSum, int opponentDamageSum)
        {
            result.AttackerDamageSum = attackerDamageSum;
            result.OpponentDamageSum = opponentDamageSum;

            attacker.Battles++;
            opponent.Battles++;

            if (result.IsVictory)
            {
                attacker.Victories++;
                opponent.Defeats++;
                attacker.Bananas += opponentDamageSum;
                opponent.Bananas += attackerDamageSum * 2;
            }
            else
            {
                attacker.Defeats++;
                opponent.Victories++;
                attacker.Bananas += opponentDamageSum;
                opponent.Bananas += attackerDamageSum * 2;
            }

            await StoreBattleHistory(attacker, opponent, result);
            await _dataContext.SaveChangesAsync();
        }

        private async Task StoreBattleHistory(User attacker, User opponent, BattleResult result)
        {
            var Battle = new Battle()
            {
                Attacker = attacker, // will automatically add AttackerId in the table
                Opponent = opponent,
                RoundsFought = result.RoundsFought,
                WinnerDamage = result.IsVictory ? result.AttackerDamageSum : result.OpponentDamageSum,
                Winner = result.IsVictory ? attacker : opponent, 
            };

            await _dataContext.Battles.AddAsync(Battle);
        }
    }
}
