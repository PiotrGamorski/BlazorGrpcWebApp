using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            var userToBeSaved = new User()
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
            };

            await _dataContext.Users.AddAsync(userToBeSaved);
            await _dataContext.SaveChangesAsync();

            return new RegisterGrpcUserResponse() { Data = request.GrpcUser.Id, Success = true, Message = "Registration successfull!" };
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
                return new LoginGrpcUserRespone() { Success = false, Message = "User Not Found" };
            else if (user != null && !await VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return new LoginGrpcUserRespone() { Success = false, Message = "Wrong password" };
            else
                return new LoginGrpcUserRespone() { Data = await CreateToken(user!), Success = true };
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

