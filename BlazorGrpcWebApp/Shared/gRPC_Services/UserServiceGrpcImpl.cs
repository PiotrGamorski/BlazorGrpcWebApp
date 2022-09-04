using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Claims;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Enums;
using Dapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

public class UserServiceGrpcImpl : UserServiceGrpc.UserServiceGrpcBase
{
    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;

    public UserServiceGrpcImpl(DataContext dataContext, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _configuration = configuration;
    }

    public override async Task<RegisterGrpcUserResponse> GrpcUserRegister(RegisterGrpcUserRequest request, ServerCallContext context)
    {
        try
        {
            await CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User()
            {
                Email = request.GrpcUser.Email,
                UserName = request.GrpcUser.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Bananas = request.GrpcUser.Bananas,
                DateOfBirth = request.GrpcUser.DateOfBirth.ToDateTime(),
                DateCreated = request.GrpcUser.DateCreated.ToDateTime(),
                IsConfirmed = request.GrpcUser.IsConfirmed,
                IsDeleted = request.GrpcUser.IsDeleted,
                Battles = 0,
                Defeats = 0,
                Victories = 0,
            };           

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            var startUnit = new UserUnit();
            startUnit.UserId = (await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.GrpcUser.Email.ToLower()))!.Id;
            startUnit.UnitId = request.StartUnitId;
            startUnit.HitPoints = (await _dataContext.Units.FirstOrDefaultAsync(u => u.Id == request.StartUnitId))!.HitPoints;

            await _dataContext.UserUnits.AddAsync(startUnit);
            await _dataContext.SaveChangesAsync();

            if (!_dataContext.UserRoles.Any())
            {
                var roleIds = await _dataContext.Roles.Select(r => r.Id).ToListAsync();
                if (roleIds.Any())
                {
                    foreach (var id in roleIds)
                    {
                        await _dataContext.AddAsync(new UserRole() { UserId = user.Id, RoleId = id });
                    }
                    await _dataContext.SaveChangesAsync();
                }
            }
            else
            {
                var userRole = await _dataContext.Roles.FirstOrDefaultAsync(ur => ur.Name == "User");
                await _dataContext.AddAsync(new UserRole() { UserId = user.Id, RoleId = userRole!.Id });
                await _dataContext.SaveChangesAsync();
            }

            var userLastActivity = new UserLastActivitie();
            userLastActivity.UserId = user.Id;
            userLastActivity.LastActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == Activity.Register))!.Id;

            await _dataContext.UserLastActivities.AddAsync(userLastActivity);
            await _dataContext.SaveChangesAsync();

            return new RegisterGrpcUserResponse() 
            { 
                Data = request.GrpcUser.Id, 
                Success = true, 
                Message = "Registration successfull!" 
            };
        }
        catch (RpcException e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public override async Task<LoginGrpcUserRespone> GrpcUserLogin(LoginGrpcUserRequest request, ServerCallContext context)
    {
        try
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.GrpcUser.Email.ToLower());

            if (user == null)
                return new LoginGrpcUserRespone() { Success = false, Message = "Invalid email address or password" };
            else if (user != null && !await VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return new LoginGrpcUserRespone() { Success = false, Message = "Invalid email address or password" };
            else
            {
                var userRoles = await _dataContext.UserRoles
                    .Include(ur => ur.Role)
                    .Where(ur => ur.UserId == user!.Id)
                    .ToListAsync();

                var loginActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == Activity.Login))!.Id;
                if (!(await _dataContext.UserLastActivities.AnyAsync(a => a.UserId == user!.Id && a.LastActivityId == loginActivityId)))
                {
                    var userLastActivity = new UserLastActivitie()
                    {
                        ExecutionDate = DateTime.Now,
                        UserId = user!.Id,
                        LastActivityId = loginActivityId,
                    };

                    await _dataContext.UserLastActivities.AddAsync(userLastActivity);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    var userLastActivity = await _dataContext.UserLastActivities.FirstOrDefaultAsync(a => a.UserId == user!.Id);
                    userLastActivity!.ExecutionDate = DateTime.Now;
                    await _dataContext.SaveChangesAsync();
                }

                return new LoginGrpcUserRespone() { Data = await CreateToken(user!, userRoles), Success = true, Message = "AuthTokenCreated" };
            }
        }
        catch (Exception e)
        {
            return new LoginGrpcUserRespone() { Success = false, Message = e.Message };
        }
    }

    public override async Task<GrpcUserExistsResponse> GrpcUserExists(GrpcUserExistsRequest request, ServerCallContext context)
    {
        var response = await _dataContext.Users.AnyAsync(user => user.Email.ToLower() == request.GrpcUser.Email.ToLower());
        return new GrpcUserExistsResponse() { Exists = response };
    }

    public override async Task<GrpcUserBananasResponse> GrpcGetUserBananas(GrpcUserBananasRequest request, ServerCallContext context)
    {
        var response = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        return new GrpcUserBananasResponse() { Bananas = response!.Bananas };
    }

    public override async Task<GrpcUserAddBananasResponse> GrpcUserAddBananas(GrpcUserAddBananasRequest request, ServerCallContext context)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        user!.Bananas += request.Amount;
        await _dataContext.SaveChangesAsync();

        return new GrpcUserAddBananasResponse { Bananas = user.Bananas };
    }

    public override async Task GrpcUserGetLeaderboard(GrpcUserGetLeaderboardRequest request, IServerStreamWriter<GrpcUserGetLeaderboardResponse> responseStream, ServerCallContext context)
    {
        var users = await _dataContext.Users.Where(u => !u.IsDeleted && u.IsConfirmed).ToListAsync();
        users = users
            .OrderByDescending(u => u.Victories)
            .ThenBy(u => u.Defeats)
            .ThenBy(u => u.DateCreated)
            .ToList();

        int rank = 1;
        // Rank is computed based on upper order
        foreach (var user in users)
        {
            await responseStream.WriteAsync(new GrpcUserGetLeaderboardResponse()
            {
                Rank = rank++,
                UserId = user.Id,
                UserName = user.UserName,
                Battles = user.Battles,
                Victories = user.Victories,
                Defeats = user.Defeats,
            });
        }
    }

    private Task CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
#pragma warning restore CA1416 // Validate platform compatibility
        return Task.CompletedTask;
    }

    private Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computePasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            foreach (var index in Enumerable.Range(0, computePasswordHash.Length))
            {
                if (computePasswordHash[index] != passwordHash[index])
                    return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
#pragma warning restore CA1416 // Validate platform compatibility
    }

    private Task<string> CreateToken(User user, List<UserRole> userRoles)
    {
        var claims = UserClaims.CreateClaims(user, userRoles);

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(jwt);
    }

    private async Task<int> GetUserIdWithDapperBy(string email)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        string query = "SELECT [Id] FROM [BlazorBattlesDb].[dbo].[Users] WHERE [Email] = @Email";
        int response = (int)(await conn.ExecuteScalarAsync(query, new { Email = email }));

       return response;
    }

    private async Task<int> GetUnitHitpointsWithDapperBy(int unitId)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        string query = "SELECT [HitPoints] FROM [BlazorBattlesDb].[dbo].[Units] WHERE [Id] = @UnitId";
        int response = (int)(await conn.ExecuteScalarAsync(query, new { UnitId = unitId }));

        return response;
    }

    private async Task CreateStartUnitWithDapper(RegisterGrpcUserRequest request)
    {
        var userId = await GetUserIdWithDapperBy(request.GrpcUser.Email);
        var unitHitPoints = await GetUnitHitpointsWithDapperBy(request.StartUnitId);

        using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        { 
            var createUserUnitQuery = @"INSERT INTO [BlazorBattlesDb].[dbo].[UserUnits] (UserId, UnitId, HitPoints) 
                                        VALUES(@UserId, @UnitId, @HitPoints)";
            await conn.ExecuteAsync(createUserUnitQuery,
            new
            {
                UserId = userId,
                UnitId = request.StartUnitId,
                HitPoints = unitHitPoints
            });
        }
    }
}