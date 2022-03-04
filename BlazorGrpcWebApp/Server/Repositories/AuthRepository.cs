using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BlazorGrpcWebApp.Server.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        public AuthRepository(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public async Task<GenericAuthResponse<string>> Login(string email, string password)
        {
            var response = new GenericAuthResponse<string>();
            var user = await _dataContext.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!await VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else 
            {
                response.Data = await CreateToken(user);
                response.Success = true;
            }
            
            return response;
        }

        public async Task<GenericAuthResponse<int>> Register(User user, string password)
        {
            if(await UserExists(user.Email))
                return new GenericAuthResponse<int>() { Success = false, Message = "User already exists."};

            try
            {
                await CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();
                return new GenericAuthResponse<int>() { Data = user.Id, Success = true, Message = "Registration successfull!" };
            }
            catch (Exception e)
            {
                return new GenericAuthResponse<int>() { Success = false, Message = e.Message };
            }
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _dataContext.Users.AnyAsync(user => user.Email.ToLower() == (email.ToLower())))
                return true;

            return false;
        }

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
                    if(computePasswordHash[index] != passwordHash[index])
                        return Task.FromResult(false);
                }
                return Task.FromResult(true);
            }
        }

        private Task<string> CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(jwt);
        }
    }
}
