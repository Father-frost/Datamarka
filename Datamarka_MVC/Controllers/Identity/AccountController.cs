using Datamarka_BLL.Contracts.Identity;
using Datamarka_DomainModel.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using ILogger = Serilog.ILogger;

namespace Datamarka_MVC.Controllers.Identity
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger logger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        const string FailureMessage = "Failed to confirm the user, wrong user id or code, please contact Your administrator";

        [HttpGet]
        [Route("confirm")]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.Information("userId and code are required fields");
                return NotFound("userId and code are required fields");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Information(FailureMessage);
                return NotFound(FailureMessage);
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded) {
                _logger.Information("Successfull email confirmation for {userId}");
                return View();
            }
            _logger.Information(FailureMessage);
            return NotFound(FailureMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


    }
}
