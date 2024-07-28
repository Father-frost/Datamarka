using Datamarka_BLL.Contracts.Identity;
using Datamarka_DomainModel.Models.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Datamarka_MVC.Controllers
{
	public class EmployeeController : Controller
	{
		private readonly IEmployeeService _employeeService;
		public EmployeeController(IEmployeeService employeeService)
		{
			_employeeService = employeeService;
		}

		// GET: EmployeeController
		public ActionResult Index()
		{
			//return View(Employees);
			return View(_employeeService.GetEmployees());
		}

		// GET: EmployeeController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: EmployeeController/Create
		//Adding an Employee to List
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Employee employee)
		{
			try
			{
				//Validation on Creating
				if (!ModelState.IsValid)
				{
					return View();
				}
				_employeeService.CreateEmployee(employee);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
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
				var employee = _employeeService.GetEmployeeById(id.Value);
				return View(employee);
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

		// GET: EmployeeController/Edit/5
		public ActionResult Edit(long? id)
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
				var editedEmployee = _employeeService.GetEmployeeById(id.Value);
				return View(editedEmployee);
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

		// POST: EmployeeController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Employee employee)
		{
			try
			{
				//Validation on Editing
				if (!ModelState.IsValid)
				{
					return View();
				}
				_employeeService.WriteEmployee(employee);
				return RedirectToAction(nameof(Index));
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
				var deletedEmployee = _employeeService.GetEmployeeById(id.Value);

				return View(deletedEmployee);
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
				_employeeService.DeleteEmployee(id);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

	}
}
