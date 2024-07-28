using Datamarka_Bll.Services.Sendgrid;
using Datamarka_BLL.Contracts.Identity;
using Datamarka_DAL;
using Datamarka_DomainModel.Models.Identity;
using Datamarka_MVC.ConfigurationSections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SendGrid.Helpers.Mail;
using System.Text;
using System.Text.Encodings.Web;
using ILogger = Serilog.ILogger;

namespace Datamarka_BLL.Services.Identity
{
    internal class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly EmailSender _emailSender;
        private readonly ILogger _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            EmailSender emailSender,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
            _logger = logger;
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }


        public async Task<User> CreateUser(UserCreateModel user)
        {
            var defaultSettings = new UserSettings {
                Address = "",
                Phone = "",
                DarkThemeEnabled = false,
            };

            var newUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                UserSettings = defaultSettings,               
            };

			_logger.Information("Start to create user!");

			await _userStore.SetUserNameAsync(newUser, user.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(newUser, user.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(newUser, user.Password);
			_logger.Information(result.ToString());


			if (result.Succeeded)
            {
                _logger.Information("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(newUser);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = $@"https://localhost:7162/confirm?userId={userId}&code={code}";

                var emailMessage = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email", emailMessage);
				return newUser;

			}
            //throw new NotImplementedException(); // todo explain!
            return null;
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
                            user.FirstName.Contains(str)
                            ||
                            user.LastName.Contains(str)
                            ||
                            user.Email.Contains(str)
                        )
                        select user;
            }

            if(role != null)
            {
                query = from user in query
                        where user.Role == role
                        select user;
            }

            var projectedQuery = from user in query
                                 select new UserBriefModel
                                 {
                                     Id = user.Id,
                                     Email = user.Email,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     Role = user.Role,
                                 };

            return projectedQuery.Skip((int)skip).Take((int)take).ToListAsync();
        }

        public async Task DeleteUser(User user)
        {

            var logins = await _userManager.GetLoginsAsync(user);
           // var rolesForUser = await _userManager.GetRolesAsync(user);

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                IdentityResult result = IdentityResult.Success;
                foreach (var login in logins)
                {
                    result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                    if (result != IdentityResult.Success)
                        break;
                }
                if (result == IdentityResult.Success)
                {
                    //Delete user's settings before user deleting
                    var repo = _unitOfWork.GetRepository<UserSettings>();
					var trackedSettings = repo
						.AsQueryable()
						.First(us => us.UserId == user.Id);

					repo.Delete(trackedSettings);
					await _unitOfWork.SaveChangesAsync();
				}
                if (result == IdentityResult.Success)
                {
                    //Delete user
                    result = await _userManager.DeleteAsync(user);
                    if (result == IdentityResult.Success)
                        transaction.Commit(); //only commit if user and all his logins/roles have been deleted  
                }
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
                FirstName = userToWrite.FirstName,
                LastName = userToWrite.LastName,
                Email = userToWrite.Email,
                Role = userToWrite.Role,
                UserSettings = defaultSettings,
            };

            repo.InsertOrUpdate(
            user => user.Id == userToWrite.Id,
            newUser
            );

            await _unitOfWork.SaveChangesAsync();
        }


        public Task<User> SetUserRole(string userId, UserRoleEnum newRole)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdatePassword(string userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUserContactData(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<UserRoleEnum?> GetUserRole(string userId)
        {
            var repo = _unitOfWork.GetRepository<User>();

            var user = await repo.AsReadOnlyQueryable().FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Role;
        }

        public Task<User> GetUserById(string userId)
        {
            var user = _userManager.FindByIdAsync(userId);

            return user;
        }
    }

}
