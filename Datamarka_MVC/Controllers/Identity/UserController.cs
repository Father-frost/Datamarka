using Datamarka_BLL.Contracts.Identity;
using Datamarka_DomainModel.Models.Identity;
using Datamarka_MVC.DataTransferObjects.Identity;
using Datamarka_MVC.RequestFilters;
using Microsoft.AspNetCore.Mvc;

namespace Datamarka_MVC.Controllers.Identity
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)]
        public async Task<IActionResult> List(int page = 0)
        {
            const int PageSize = 3;
            var allUsers = await _userService.FetchUsers();
            var count = allUsers.Count;
            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            return View(await _userService.FetchUsers(skip: (page * PageSize), take: PageSize));
        }

        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)]
        public async Task<IActionResult> EmployeeList(int page = 0)
        {
            const int PageSize = 3;
            var allUsers = await _userService.FetchUsers(role: UserRoleEnum.Employee);
            var count = allUsers.Count;
            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            return View(await _userService.FetchUsers(skip: (page * PageSize), take: PageSize, role: UserRoleEnum.Employee));
        }

        // GET: EmployeeController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)] // Micorosft Identity's attribute - uses Claims and dynamic Roles, too complicated for our usecase
        public async Task<IActionResult> Create(UserBriefDTO user)
        {
            var newUserModel = new UserCreateModel
            {
                UserName = user.UserName,
                Role = user.Role,
                Password = user.Password,
            };


            try
            {
                if (user.Password == user.ConfirmPassword)
                {
                    if (!ModelState.IsValid)
                    {
                        return View();
                    }
                    var newUser = await _userService.CreateUser(newUserModel);
                    if (newUser == null)
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound("Пароль не совпадает с подтвеждением!");
                }
                return RedirectToAction(nameof(List));
            }
            catch
            {
                return View();

            }

        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserBriefDTO user)
        {

            var newUserModel = new UserCreateModel
            {
                UserName = user.UserName,
                Role = UserRoleEnum.Customer,
                Password = user.Password,
            };

            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                if (user.Password == user.ConfirmPassword)
                {
                    var newUser = await _userService.CreateUser(newUserModel);


                    if (newUser == null)
                    {
                        return NotFound("Possibly bad password or duplicated user!");
                    }
                }
                else
                {
                    return NotFound("Пароль не совпадает с подтвеждением!");
                }
                return RedirectToAction("index", "home");
            }
            catch
            {
                return View();

            }

        }

        [HttpGet]
        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)]
        public async Task<IActionResult> SearchForUsers(
            [FromQuery] UserSearchDTO searchParams)
        {
            var users = await _userService.FetchUsers(
                searchParams.Skip,
                searchParams.Take,
                searchParams.SearchString,
                searchParams.Role);

            if (users.Count == 0)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpGet]
        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)]
        public async Task<IActionResult> Delete(long id)
        {
            if (id == null)
            {
                return NotFound("Empty id supplied!");
            }
            try
            {
                var deletedUser = await _userService.GetUserById(id);

                return View(deletedUser);
            }
            catch
            {
                return NotFound("No such record found!");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            if (id > 0)
            {
                try
                {


                    await _userService.DeleteUser(id);
                    return RedirectToAction(nameof(List));

                }
                catch
                {
                    return NotFound("No such record found!");
                }
            }
            else
            {
                return NotFound("Empty id supplied!");
            }
        }

        [HttpGet]
        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)]
        public async Task<IActionResult> Edit(long id)
        {
            if (id > 0)
            {
                try
                {
                    var editedUser = await _userService.GetUserById(id);

                    return View(editedUser);
                }
                catch
                {
                    return NotFound("No such record found!");
                }
            }
            else
            {
                return NotFound("Empty id supplied!");

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Administrator)]
        public async Task<ActionResult> Edit(UserBriefDTO user)
        {
            var updateUserModel = new UserCreateModel
            {
                UserName = user.UserName,
                Role = user.Role,
                Password = user.Password,
            };
            try
            {
                //Validation on Editing
                if (!ModelState.IsValid)
                {
                    return View();
                }

                await _userService.WriteUser(updateUserModel);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));


            }
        }
    }
}
