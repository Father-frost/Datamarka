using Datamarka_BLL.Contracts.Identity;
using Datamarka_DomainModel.Models.Identity;
using Datamarka_MVC.DataTransferObjects.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ILogger = Serilog.ILogger;

namespace Datamarka_MVC.Controllers.Identity
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        public UserRoleEnum Role { get; set; }

        public AccountController(
            ILogger logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public ActionResult Index()
        {
            //return View(Employees);
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userService.GetUserByUserName(model.UserName);
                if (user != null)
                {
                    await Authenticate(model.UserName); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var user = await _userService.GetUserByUserName(userName);
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim("ActualRole", user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userService.GetUserByUserName(model.UserName);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    await _userService.CreateUser(model);

                    await Authenticate(model.UserName); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Account");
        }

    }
}
