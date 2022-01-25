using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Data;
using BlazorGrpcWebApp.Shared.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

public class UserServiceGrpcImpl : UserServiceGrpc.UserServiceGrpcBase
{
    private readonly DataContext _dataContext;
    public UserServiceGrpcImpl(DataContext dataContext)
    {
        _dataContext = dataContext;
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
                return new LoginGrpcUserRespone() { Data = request.GrpcUser.Id.ToString(), Success = true };
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
}

