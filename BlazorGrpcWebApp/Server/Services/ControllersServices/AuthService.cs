using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Claims;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Enums;
using BlazorGrpcWebApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace BlazorGrpcWebApp.Server.Services.ControllersServices
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(DataContext dataContext, IConfiguration configuration, IEmailService emailService)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<GenericAuthResponse<string>> Login(string email, string password)
        {
            var response = new GenericAuthResponse<string>();
            var user = await _dataContext!.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());
            var roles = await _dataContext!.Roles.ToListAsync();
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!await VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong email or password.";
            }
            else
            {
                var userRoles = await _dataContext.UserRoles
                    .Include(ur => ur.Role)
                    .Where(ur => ur.UserId == user!.Id)
                    .ToListAsync();
                response.Data = await CreateToken(user, userRoles);
                response.Success = true;

                var loginActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == Activity.Login))!.Id;
                if (!(await _dataContext.UserLastActivities.AnyAsync(a => a.UserId == user.Id && a.LastActivityId == loginActivityId)))
                {
                    var userLastActivity = new UserLastActivitie()
                    {
                        ExecutionDate = DateTime.Now,
                        UserId = user.Id,
                        LastActivityId = loginActivityId,
                    };

                    await _dataContext.UserLastActivities.AddAsync(userLastActivity);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    var userLastActivity = await _dataContext.UserLastActivities.FindAsync(user);
                    userLastActivity!.ExecutionDate = DateTime.Now;
                    await _dataContext.SaveChangesAsync();
                }
            }

            return response;
        }

        public async Task<GenericAuthResponse<int>> Register(User user, string password, int startUnitId)
        {
            if (await UserEmailExists(user.Email))
                return new GenericAuthResponse<int>() { Success = false, Message = "User already exists." };

            try
            {
                await CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                var verificationCode = GenerateVerificationCode(6);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.VerificationCode = verificationCode;

                await _dataContext!.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();

                await _emailService.SendEmail(new EmailDto()
                {
                    To = user.Email,
                    Subject = "Blazor Battles Verification Code",
                    Body = $"<h3>Hi {user.UserName}!</h3><p>Welcome to Blazor Battles. To complete your registration use verification code: {verificationCode}</p>"
                });

                if (!_dataContext.UserRoles.Any())
                {
                    var roleIds = _dataContext.Roles.Select(r => r.Id).ToList();
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

                var userUnit = new UserUnit();
                userUnit.UserId = (await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email))!.Id;
                userUnit.UnitId = startUnitId;
                userUnit.HitPoints = (await _dataContext.Units.FirstOrDefaultAsync(u => u.Id == startUnitId))!.HitPoints;

                await _dataContext.UserUnits.AddAsync(userUnit);
                await _dataContext.SaveChangesAsync();

                var userLastActivity = new UserLastActivitie();
                userLastActivity.UserId = user.Id;
                userLastActivity.LastActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == Activity.Register))!.Id;

                await _dataContext.UserLastActivities.AddAsync(userLastActivity);
                await _dataContext.SaveChangesAsync();

                return new GenericAuthResponse<int>() { Data = user.Id, Success = true, Message = "Registration successfull!" };
            }
            catch (Exception e)
            {
                return new GenericAuthResponse<int>() { Success = false, Message = e.Message };
            }
        }

        public async Task<GenericAuthResponse<bool>> Verify(VerifyCodeRequestDto request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == request.UserEmail);

            if (user == null)
                return new GenericAuthResponse<bool> { Success = false, Message = "User doesn't exist" };

            if (user.IsVerified)
                return new GenericAuthResponse<bool> { Success = true, Message = "User already veryfied" };

            int dateComparison = DateTime.Compare(user.VerificationCodeExpireDate, DateTime.Now);
            if (dateComparison > 0)
            {
                if (user.VerificationCode.ToUpper() == request.VerificationCode!.ToUpper())
                {
                    user.IsVerified = true;
                    await _dataContext.SaveChangesAsync();

                    var userLastActivity = new UserLastActivitie();
                    userLastActivity.UserId = user.Id;
                    userLastActivity.LastActivityId = (await _dataContext.LastActivities.FirstOrDefaultAsync(a => a.ActivityType == Activity.Verify))!.Id;

                    await _dataContext.UserLastActivities.AddAsync(userLastActivity);
                    await _dataContext.SaveChangesAsync();

                    return new GenericAuthResponse<bool> { Success = true,  Data = true, Message = "Verification completed" };
                }
                else return new GenericAuthResponse<bool> { Success = false, Message = "Invalid verification code" };
            }
            else return new GenericAuthResponse<bool> { Success = false, Message = "Verification code has expired" };
        }

        public async Task<bool> UserEmailExists(string email)
        {
            return await _dataContext!.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UserNameExists(string userName)
        {
            return await _dataContext.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }


        #region Private Methods
        private Task CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            return Task.CompletedTask;
        }

        private Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
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

        private string GenerateVerificationCode(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion
    }
}
