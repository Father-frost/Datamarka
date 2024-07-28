using Datamarka_MVC.DataTransferObjects.Identity;
using Datamarka_DomainModel.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;
using System.Collections.Generic;
using Datamarka_MVC.RequestFilters;
using System.Security.Claims;
using Datamarka_BLL.Contracts.Identity;
using Microsoft.AspNetCore.Authorization;
using static System.Net.WebRequestMethods;
using Datamarka_MVC.Models;

namespace Datamarka_MVC.Controllers.Identity
{
    public class LoginController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        public UserRoleEnum Role { get; set; }

        public LoginController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger logger)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            //return View(Employees);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginData)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (!ModelState.IsValid)
            {
                return NotFound(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(loginData.Email, loginData.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.Information($@"User {loginData.Email} has logged in.");

                // todo get the user's role and put it into Claims, so it will be easier for filters to check access
                var currentUser = HttpContext.User;
                var userIdClaim = currentUser.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier);
                var userService = HttpContext.RequestServices.GetRequiredService<IUserService>();
                var actualRole = await userService.GetUserRole(userIdClaim.Value);
                var user = await _userManager.FindByIdAsync(userIdClaim.Value);
                await _userManager.AddClaimAsync(user, new Claim("ActualRole", actualRole.ToString()));
                await _userManager.UpdateAsync(user);

                if (actualRole == UserRoleEnum.Administrator)
                {
                    return RedirectToAction("AdminPanel", "Home");
                }
                if (actualRole == UserRoleEnum.Manager)
                {
                    return RedirectToAction("ManagerPanel", "Home");
                }
                if (actualRole == UserRoleEnum.Employee)
                {
                    return RedirectToAction("EmployeePanel", "Home");
                }
                if (actualRole == UserRoleEnum.Customer)
                {
                    return RedirectToAction("CustomerPage", "Home");
                }

                return RedirectToAction("index", "home");
            }

            if (result.IsLockedOut)
            {
                return Unauthorized("The user has been locked out, please contact Your administrator");
            }
            
            if (result.IsNotAllowed)
            {
                return Unauthorized("The user is not allowed to log in at the moment");
            }

            return Unauthorized("Email or/and Password are incorrect, please try again");
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            returnUrl = $@"https://localhost:7162/";
            provider = "Google";
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnPath = returnUrl,
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                var currentUser = HttpContext.User;
                var userIdClaim = currentUser.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier);
                var userService = HttpContext.RequestServices.GetRequiredService<IUserService>();
                var actualRole = await userService.GetUserRole(userIdClaim.Value);
                var user = await _userManager.FindByIdAsync(userIdClaim.Value);
                await _userManager.AddClaimAsync(user, new Claim("ActualRole", actualRole.ToString()));
                await _userManager.UpdateAsync(user);

                if (actualRole == UserRoleEnum.Administrator)
                {
                    return RedirectToAction("AdminPanel", "Home");
                }
                if (actualRole == UserRoleEnum.Manager)
                {
                    return RedirectToAction("ManagerPanel", "Home");
                }
                if (actualRole == UserRoleEnum.Employee)
                {
                    return RedirectToAction("EmployeePanel", "Home");
                }
                if (actualRole == UserRoleEnum.Customer)
                {
                    return RedirectToAction("CustomerPage", "Home");
                }

                return RedirectToAction("index", "home");
            //return LocalRedirect(returnUrl);
        }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await _userManager.FindByEmailAsync(email);


                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }

    }
}
