﻿using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models.UI_Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Server.Services.ControllersServices
{
    public class BattleService : IBattleService
    {
        public BattleService() { }

        public async Task Fight(DataContext dataContext, User attacker, User opponent, BattleResult result)
        {
            var attackerArmy = await dataContext.UserUnits
                .Where(u => u.UserId == attacker.Id && u.HitPoints > 0)
                .Include(u => u.Unit)
                .ToListAsync();
            // one can include Unit as it appears in UserUnit class
            var opponentArmy = await dataContext.UserUnits
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
                await FinishFight(dataContext, attacker, opponent, result, attackerDamageSum, opponentDamageSum);
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

        private async Task FinishFight(DataContext dataContext, User attacker, User opponent, BattleResult result,
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

            await StoreBattleHistory(dataContext, attacker, opponent, result);
            await dataContext.SaveChangesAsync();
        }

        private async Task StoreBattleHistory(DataContext dataContext, User attacker, User opponent, BattleResult result)
        {
            var Battle = new Battle()
            {
                Attacker = attacker, // will automatically add AttackerId in the table
                Opponent = opponent,
                RoundsFought = result.RoundsFought,
                WinnerDamage = result.IsVictory ? result.AttackerDamageSum : result.OpponentDamageSum,
                Winner = result.IsVictory ? attacker : opponent,
            };

            await dataContext.Battles.AddAsync(Battle);
        }
    }
}
