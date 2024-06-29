using DocsManager.Models.Dto;

namespace DocsManager.Services.User;

public interface IUserService
{
    public Task<bool> ValidateUser(Guid userId);
    public Task<bool> InsertUser(Guid userId, UserPostDto userPost, string userName, string surName);
    public Task<bool> UpdateUser(Guid userId, UserPostDto userPost, string userName, string surName);
    public Task<Models.User?> GetUser(Guid userId);
}