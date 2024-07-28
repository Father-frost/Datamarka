using Datamarka_BLL.Contracts;
using Datamarka_BLL.Contracts.Identity;
using Datamarka_DomainModel.Models.ECommerce;
using Datamarka_DomainModel.Models.Identity;
using Datamarka_MVC.RequestFilters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ILogger = Serilog.ILogger;



namespace Datamarka_MVC.Controllers
{
	public class OrderController : Controller
	{
		private readonly IOrderService _orderService;
		private readonly UserManager<User> _userManager;
		private readonly ILogger _logger;

		public OrderController(
			IOrderService orderService,
			UserManager<User> userManager,
			ILogger logger
			)
		{
			_orderService = orderService;
			_userManager = userManager;
			_logger = logger;
		}

        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Customer)]
        public async Task<IActionResult> List(int page = 0)
		{
			var allOrders = new List<OrderBriefModel>();
			var orders = new List<OrderBriefModel>();

            if (User.Identity.IsAuthenticated)
			{
				const int PageSize = 3;
				var currentUser = HttpContext.User;
				var userIdClaim = currentUser.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier);
                var actualRole = User.Claims.FirstOrDefault(cl => cl.Type == "ActualRole").Value.ToString();
				if (actualRole == UserRoleEnum.Customer.ToString())
				{
					allOrders = await _orderService.FetchOrders(searchString: userIdClaim.Value.ToString());
					orders = await _orderService.FetchOrders(skip: (page * PageSize), take: PageSize, searchString: userIdClaim.Value.ToString());
				}
				else
				{
                    allOrders = await _orderService.FetchOrders();
                    orders = await _orderService.FetchOrders(skip: (page * PageSize), take: PageSize);
                }
                var count = allOrders.Count;
				ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

				ViewBag.Page = page;

				return View(orders);
			}
			return NotFound("Please, sign in to leave an order!");
		}

        public async Task<IActionResult> ListForManager(int page = 0)
		{
			var allOrders = new List<OrderBriefModel>();
			var orders = new List<OrderBriefModel>();

            if (User.Identity.IsAuthenticated)
			{
				const int PageSize = 3;
				var currentUser = HttpContext.User;
				var userIdClaim = currentUser.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier);
                var actualRole = User.Claims.FirstOrDefault(cl => cl.Type == "ActualRole").Value.ToString();
				if (actualRole == UserRoleEnum.Manager.ToString() || actualRole == UserRoleEnum.Administrator.ToString())
				{
                    allOrders = await _orderService.FetchOrders();
                    orders = await _orderService.FetchOrders(skip: (page * PageSize), take: PageSize);
                }
                var count = allOrders.Count;
				ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

				ViewBag.Page = page;

				return View(orders);
			}
			return NotFound("Please, sign in to leave an order!");
		}

        [RoleBasedAuthorizationFilter(Role = UserRoleEnum.Employee)]
        public async Task<IActionResult> ListForEmployee(int page = 0)
        {
            var allOrders = new List<OrderBriefModel>();
            var orders = new List<OrderBriefModel>();

            if (User.Identity.IsAuthenticated)
            {
                const int PageSize = 3;
                var currentUser = HttpContext.User;
                var userIdClaim = currentUser.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier);
                var actualRole = User.Claims.FirstOrDefault(cl => cl.Type == "ActualRole").Value.ToString();
                if (actualRole == UserRoleEnum.Employee.ToString())
                {
                    allOrders = await _orderService.FetchOrdersForEmployee();
                    orders = await _orderService.FetchOrdersForEmployee(skip: (page * PageSize), take: PageSize);
                }
                var count = allOrders.Count;
                ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;

                return View(orders);
            }
            return NotFound("Please, sign in to leave an order!");
        }

        public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(OrderDTO order)
		{
			if (User.Identity.IsAuthenticated)
			{
				var currentUser = HttpContext.User;
				var userIdClaim = currentUser.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier);
				var user = await _userManager.FindByIdAsync(userIdClaim.Value);

				var newOrderModel = new OrderBriefModel
				{
					User = user,
					ProductId = order.ProductId,
					ProdDate = DateTime.Now,
					WarrantDate = DateTime.Now,
					Status = OrderStatusEnum.Placed,
				};

				try
				{
					//Validation on Creating
					if (!ModelState.IsValid)
					{
						return View();
					}
					_orderService.CreateOrder(newOrderModel);
					return RedirectToAction(nameof(List));
				}
				catch
				{
					return View();
				}
			}
			return NotFound("Please, sign in to leave an order!");
		}


		// GET: EmployeeController/Details/5
		public ActionResult Details(long? id)
		{
			if (!id.HasValue)
			{
				return NotFound("Empty id supplied!");
			}

			try
			{
				var order = _orderService.GetOrderById(id.Value);
				return View(order);
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

		public async Task<IActionResult> Edit(long? id)
		{
			if (!id.HasValue)
			{
				return NotFound("Empty id supplied!");
			}

			try
			{
				//Validation on Editing
				if (!ModelState.IsValid)
				{
					return View();
				}
				var editedOrder = await _orderService.GetOrderById(id.Value);
				return View(editedOrder);
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

		// POST: EmployeeController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(OrderBriefModel order)
		{

			try
			{
				//Validation on Editing
				if (!ModelState.IsValid)
				{
					return View();
				}
				await _orderService.WriteOrder(order);
				return RedirectToAction(nameof(List));
			}
			catch
			{
				return RedirectToAction(nameof(Index));
			}
		}

		// GET: EmployeeController/Delete/5
		[HttpGet]
		public ActionResult Delete(long? id)
		{
			if (!id.HasValue)
			{
				return NotFound("Empty id supplied!");
			}
			try
			{
				var deletedOrder = _orderService.GetOrderById(id.Value);

				return View(deletedOrder);
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

		// POST: EmployeeController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(long id)
		{
			try
			{
				_orderService.DeleteOrder(id);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

	}
}
