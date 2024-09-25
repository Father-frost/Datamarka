using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_BLL.Contracts.Identity
{
    public interface IUserService : IService
    {
        Task<List<UserBriefModel>> FetchUsers(long skip = 0, long take = 20, string? searchString = null, UserRoleEnum? role = null);

        Task<User> CreateUser(UserCreateModel user);

        Task DeleteUser(long userId);

        Task WriteUser(UserCreateModel user);

        Task<User> GetUserById(long userId);

        Task<User> GetUserByUserName(string login);

        Task<User> UpdateUserContactData(User user);

        
        Task<User> SetUserRole(long userId, UserRoleEnum newRole);

        Task<UserRoleEnum?> GetUserRole(long userId);

        Task<User> UpdatePassword(long userId, string newPassword);
    }
}
