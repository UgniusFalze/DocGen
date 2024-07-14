using DocsManager.Models;
using DocsManager.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Services.User;

public class UserService(DocsManagementContext context) : IUserService
{
    public async Task<bool> ValidateUser(Guid userId)
    {
        return await context.Users.AnyAsync(cxUser => cxUser.UserId == userId);
    }

    public async Task<bool> InsertUser(Guid userId, UserPostDto userPost, string userName, string surName)
    {
        if (await ValidateUser(userId)) return false;

        context.Users.Add(new Models.User
        {
            Address = userPost.Address,
            BankName = userPost.BankName,
            BankNumber = userPost.BankNumber,
            FirstName = userName,
            LastName = surName,
            FreelanceWorkId = userPost.FreelanceWorkId,
            PersonalId = userPost.PersonalId,
            UserId = userId
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUser(Guid userId, UserPostDto userPost, string userName, string surName)
    {
        var modifiedUser = new Models.User
        {
            UserId = userId,
            FirstName = userName,
            LastName = surName,
            Address = userPost.Address,
            BankName = userPost.BankName,
            BankNumber = userPost.BankNumber,
            FreelanceWorkId = userPost.FreelanceWorkId,
            PersonalId = userPost.PersonalId
        };

        context.Entry(modifiedUser).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ValidateUser(userId))
                return false;
            throw;
        }

        return true;
    }

    public async Task<Models.User?> GetUser(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        return user;
    }
}