using Datamarka_BLL.Contracts.Identity;
using Datamarka_DAL;
using Datamarka_DomainModel.Models.Identity;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Datamarka_BLL.Services.Identity
{
    internal class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<User> CreateUser(UserCreateModel user)
        {
            var repo = _unitOfWork.GetRepository<User>();

            var defaultSettings = new UserSettings
            {
                Address = "",
                Phone = "",
                DarkThemeEnabled = false,
            };

            var newUser = new User
            {
                UserName = user.UserName,
                Role = user.Role,
                Password = user.Password,
                UserSettings = defaultSettings,
            };

            _logger.Information("Start to create user!");

            var trackedUser = repo.Create(newUser);

            await _unitOfWork.SaveChangesAsync();

            return trackedUser;
        }

        public Task<List<UserBriefModel>> FetchUsers(long skip = 0, long take = 20, string? searchString = null, UserRoleEnum? role = null)
        {
            var repo = _unitOfWork.GetRepository<User>();

            var query = repo.AsReadOnlyQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                var searchStrings = searchString.Split(' ');

                query = from user in query
                        where searchStrings.All(str =>
                            user.UserName.Contains(str)
                        )
                        select user;
            }

            if (role != null)
            {
                query = from user in query
                        where user.Role == role
                        select user;
            }

            var projectedQuery = from user in query
                                 select new UserBriefModel
                                 {
                                     Id = user.Id,
                                     UserName = user.UserName,
                                     Role = user.Role,
                                 };

            return projectedQuery.Skip((int)skip).Take((int)take).ToListAsync();
        }

        public async Task DeleteUser(long userId)
        {
            var repo = _unitOfWork.GetRepository<User>();
            var trackedUser = repo
            .AsQueryable()
                .First(us => us.Id == userId);

            try
            {
                repo.Delete(trackedUser);
                var save = await _unitOfWork.SaveChangesAsync();
                if (save > 0)
                {
                    var repo2 = _unitOfWork.GetRepository<UserSettings>();
                    var trackedSettings = repo2
                        .AsQueryable()
                        .First(us => us.UserId == userId);
                    repo2.Delete(trackedSettings);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task WriteUser(UserCreateModel userToWrite)
        {
            var repo = _unitOfWork.GetRepository<User>();

            var defaultSettings = new UserSettings
            {
                Address = "",
                Phone = "",
                DarkThemeEnabled = false,
            };

            var newUser = new User
            {
                UserName = userToWrite.UserName,
                Role = userToWrite.Role,
                UserSettings = defaultSettings,
                Password = userToWrite.Password,
            };

            repo.InsertOrUpdate(
            user => user.Id == userToWrite.Id,
            newUser
            );

            await _unitOfWork.SaveChangesAsync();
        }


        public Task<User> SetUserRole(long userId, UserRoleEnum newRole)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdatePassword(long userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUserContactData(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<UserRoleEnum?> GetUserRole(long userId)
        {
            var repo = _unitOfWork.GetRepository<User>();

            var user = await repo.AsReadOnlyQueryable().FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Role;
        }

        public async Task<User> GetUserById(long userId)
        {
            var repo = _unitOfWork.GetRepository<User>();

            var user = await repo.AsReadOnlyQueryable()
                .FirstOrDefaultAsync(us => us.Id == userId);

            return user;
        }

        public async Task<User> GetUserByUserName(string login)
        {
            var repo = _unitOfWork.GetRepository<User>();

            var user = await repo.AsReadOnlyQueryable()
                .FirstOrDefaultAsync(us => us.UserName == login);

            return user;
        }
    }

}
