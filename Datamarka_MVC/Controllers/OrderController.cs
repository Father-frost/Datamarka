using Datamarka_BLL.Contracts;
using Datamarka_BLL.Contracts.Identity;
using Datamarka_DomainModel.Models.ECommerce;
using Datamarka_DomainModel.Models.Identity;
using Datamarka_MVC.RequestFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SuperSimpleTcp;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using ILogger = Serilog.ILogger;



namespace Datamarka_MVC.Controllers
{
	public class OrderController : Controller
	{
		private readonly IOrderService _orderService;
		private readonly IUserService _userService;
		private readonly IProductService _productService;
		private readonly ILogger _logger;
		private SimpleTcpClient simpleClient;
		Regex regex = null;
		Match match = null;
		string lastcode = "0000000";
		string pattern = @"0104\d{12}21.{8,14}93.{4}";  //Шаблон для поиска кода маркировки

		public OrderController(
			IOrderService orderService,
			IUserService userService,
			IProductService productService,
			ILogger logger
			)
		{
			_orderService = orderService;
			_userService = userService;
			_productService = productService;
			_logger = logger;
		}

		public async Task<IActionResult> List(int page = 0)
		{
			var allOrders = new List<OrderBriefModel>();
			var orders = new List<OrderBriefModel>();

			if (User.Identity.IsAuthenticated)
			{
				const int PageSize = 3;
				allOrders = await _orderService.FetchOrders();
				orders = await _orderService.FetchOrders(skip: (page * PageSize), take: PageSize);
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
				//var currentUser = HttpContext.User;
				//var userIdClaim = currentUser.Claims.FirstOrDefault(cl => cl.Type == ClaimTypes.NameIdentifier);
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
			SelectList products = new SelectList(_productService.GetProducts(), "Id", "Name");
			ViewBag.Products = products;
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
				int.TryParse(userIdClaim?.Value, out int userId);
				//var user = await _userService.GetUserById(userId);

				var newOrderModel = new OrderBriefModel
				{
					UserId = userId,
					ProductId = order.ProductId,
					ProdDate = order.ProdDate,
					WarrantDate = order.WarrantDate,
					Batch = order.Batch,
					Status = OrderStatusEnum.Placed,
				};

				try
				{
					//Validation on Creating
					if (!ModelState.IsValid)
					{
						return View();
					}
					await _orderService.CreateOrder(newOrderModel);
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
				regex = new Regex(pattern);
				ConnectToServer();
				//Validation on Editing
				if (!ModelState.IsValid)
				{
					return View();
				}
				SelectList products = new SelectList(_productService.GetProducts(), "Id", "Name");
				ViewBag.Products = products;
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
		public async Task<IActionResult> Edit(Order order)
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
				return RedirectToAction(nameof(List));
			}
		}

		// GET: EmployeeController/Delete/5
		[HttpGet]
		public async Task<IActionResult> Delete(long? id)
		{
			if (!id.HasValue)
			{
				return NotFound("Empty id supplied!");
			}
			try
			{
				var deletedOrder = await _orderService.GetOrderById(id.Value);

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
				return RedirectToAction(nameof(List));
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}




		private void ConnectToServer(bool reconnect = true)
		{
			if (reconnect)
			{
				simpleClient?.Dispose();
			}

			simpleClient = new SimpleTcpClient("10.0.1.214", 50010);



			simpleClient.Settings = new SimpleTcpClientSettings()
			{
				ConnectTimeoutMs = 1500,
				IdleServerTimeoutMs = 0//requestTimeout
			};

			simpleClient.Events.Connected += Connected;
			simpleClient.Events.Disconnected += Disconnected;
			simpleClient.Events.DataReceived += DataReceived;
			//simpleClient.Keepalive.EnableTcpKeepAlives = true;

			try
			{
				simpleClient.ConnectWithRetries(1500);
				if (!simpleClient.IsConnected)
				{
					_logger.Information("[response] НЕ удается подключиться к камере!");
				}
			}
			catch (Exception)
			{
				_logger.Information("[response] НЕ удается подключиться к камере!");
			}
		}

		public void Connected(object sender, SuperSimpleTcp.ConnectionEventArgs e)
		{
			_logger.Information("[response] Камера подключена!");
		}

		public void Disconnected(object sender, SuperSimpleTcp.ConnectionEventArgs e)
		{
			_logger.Information("[response] Камера отключена!");

			//переподключение
			// ConnectToServer(true);

		}


		public void DataReceived(object sender, SuperSimpleTcp.DataReceivedEventArgs e)
		{
			_logger.Information(Encoding.UTF8.GetString(e.Data));

			string response = Encoding.UTF8.GetString(e.Data);
			getResponse(response);

		}

		//Обработка ответа камеры
		public async void getResponse(string response)
		{
			match = regex.Match(response);

			while (match.Success)
			{
				string str = match.Groups[0].Value;
				//str = str.Substring(0, 33); //Вырезаем первые 34 символа
				if (!str.Contains(lastcode))
				{
					lastcode = str;
					if (str.Contains(@"\u001d"))
					{
						string st = str.Replace(@"\u001d", @"");  //Замена символа GS
						str = st;
					}

					////Запись кода
					//Invoke(new AddMessageDelegate(LogAdd), new object[] { str + "\n" });
					//InsertCode(str);  //Запись кода в БД
					//counter++;  //Увеличение счетчика
					//Invoke(new AddMessageDelegate(SetCounter), new object[] { counter.ToString() });

				}
				match = match.NextMatch();
			}

		}

	}
}
