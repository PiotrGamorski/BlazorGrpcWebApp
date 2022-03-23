using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace BlazorGrpcWebApp.Shared.gRPC_Services
{
    public class BattleServiceGrpcImpl : BattleServiceGrpc.BattleServiceGrpcBase
    {
        private readonly DataContext _dataContext;
        public BattleServiceGrpcImpl(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override async Task<GrpcStartBattleResponse> GrpcStartBattle(GrpcStartBattleRequest request, ServerCallContext context)
        {
            var attacker = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == request.AuthUserId);
            var opponent = await _dataContext.Users.FindAsync(request.OppenentId);
            if (opponent == null || opponent.IsDeleted)
                throw new RpcException(new Status(StatusCode.NotFound, "Opponent not available"));

            var result = new BattleResult();
            await Fight(attacker!, opponent, result);

            if (result.Log.Count > 0)
            {
                await StoreBattleLogs(attacker!, opponent, result);
                await _dataContext.SaveChangesAsync();
            }

            var grpcStartBattleResponse = new GrpcStartBattleResponse();
            grpcStartBattleResponse.BattleResult = result.IsVictory;
            grpcStartBattleResponse.Logs.Add(result.Log);
            return grpcStartBattleResponse;
        }

        private async Task Fight(User attacker, User opponent, BattleResult result)
        {
            var attackerArmy = await _dataContext.UserUnits
                .Where(u => u.UserId == attacker.Id && u.HitPoints > 0)
                .Include(u => u.Unit)
                .ToListAsync();

            var opponentArmy = await _dataContext.UserUnits
                .Where(u => u.UserId == opponent.Id)
                .Include(u => u.Unit)
                .ToListAsync();

            var attackerDamageSum = 0;
            var opponentDamageSum = 0;
            var currentRound = 0;
            while (attackerArmy.Count() > 0 && opponentArmy.Count() > 0)
            {
                currentRound++;
                if(currentRound % 2 != 0)
                    attackerDamageSum += await FightRound(attacker, opponent, attackerArmy, opponentArmy, result);
                else
                    opponentDamageSum += await FightRound(opponent, attacker, opponentArmy, attackerArmy, result);
            }

            result.IsVictory = opponentArmy.Count() == 0;
            result.RoundsFought = currentRound;

            if(result.RoundsFought > 0)
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

        private async Task StoreBattleLogs(User attacker, User opponent, BattleResult result)
        {
            var currentBattle = await _dataContext.Battles
                .OrderBy(b => b.Id)
                .LastOrDefaultAsync(b => b.AttackerId == attacker.Id && b.OpponentId == opponent.Id);

            var battleLogsToDelete = await _dataContext.BattleLogs
                .Where(b => b.BattleId != currentBattle!.Id && b.AttackerId == attacker.Id && b.OpponentId == opponent.Id)
                .ToListAsync();

            _dataContext.BattleLogs.RemoveRange(battleLogsToDelete);
            await _dataContext.SaveChangesAsync();

            foreach (var log in result.Log)
            {
                await _dataContext.BattleLogs.AddAsync(new BattleLog() 
                { 
                    Battle = currentBattle!,
                    Attacker = attacker,
                    Opponent = opponent,
                    Log = log
                });
            }
        }
    }
}   
