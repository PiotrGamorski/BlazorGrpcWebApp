using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BlazorGrpcWebApp.Server.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;
        public AuthRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
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
                response.Data = user.Id.ToString();
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
    }
}
