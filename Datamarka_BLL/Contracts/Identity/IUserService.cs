using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_BLL.Contracts.Identity
{
    public interface IUserService : IService
    {
        Task<List<UserBriefModel>> FetchUsers(long skip = 0, long take = 20, string? searchString = null, UserRoleEnum? role = null);

        Task<User> CreateUser(UserCreateModel user);

        Task DeleteUser(User user);

        Task WriteUser(UserCreateModel user);

        Task<User> GetUserById(string userId);

        Task<User> UpdateUserContactData(User user);

        
        Task<User> SetUserRole(string userId, UserRoleEnum newRole);

        Task<UserRoleEnum?> GetUserRole(string userId);

        Task<User> UpdatePassword(string userId, string newPassword);
    }
}
