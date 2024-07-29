using Datamarka_BLL.Contracts;
using Datamarka_DomainModel.Models.ECommerce;
using Microsoft.AspNetCore.Mvc;


namespace Datamarka_MVC.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _productService;
		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		// GET: ProductController
		public ActionResult Index()
		{
			//return View(Products);
			return View(_productService.GetProducts());
		}

		// GET: ProductController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: ProductController/Create
		//Adding an Product to List
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Product product)
		{
			try
			{
				//Validation on Creating
				if (!ModelState.IsValid)
				{
					return View();
				}
				_productService.CreateProduct(product);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}


		// GET: ProductController/Details/5
		public ActionResult Details(long? id)
		{
			if (!id.HasValue)
			{
				return NotFound("Empty id supplied!");
			}

			try
			{
				var product = _productService.GetProductById(id.Value);
				return View(product);
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
				var editedProduct = _productService.GetProductById(id.Value);
				return View(editedProduct);
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

		// POST: ProductController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Product product)
		{
			try
			{
				//Validation on Editing
				if (!ModelState.IsValid)
				{
					return View();
				}
				_productService.WriteProduct(product);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return RedirectToAction(nameof(Index));
			}
		}

		// GET: ProductController/Delete/5
		[HttpGet]
		public ActionResult Delete(long? id)
		{
			if (!id.HasValue)
			{
				return NotFound("Empty id supplied!");
			}
			try
			{
				var deletedProduct = _productService.GetProductById(id.Value);

				return View(deletedProduct);
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

		// POST: ProductController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(long id)
		{
			try
			{
				_productService.DeleteProduct(id);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return NotFound("No such record found!");
			}
		}

	}
}
