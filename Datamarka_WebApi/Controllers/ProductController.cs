using Datamarka_BLL.Contracts;
using Datamarka_DomainModel.Models.ECommerce;
using Microsoft.AspNetCore.Mvc;


namespace Datamarka_WebApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;
		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		// POST: ProductController/Create
		//Adding an Product to List
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Product product)
		{
			try
			{
				await _productService.CreateProduct(product);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return NotFound();
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
				return Ok(product);
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
				var editedProduct = _productService.GetProductById(id.Value);
				return Ok(editedProduct);
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

				return Ok(deletedProduct);
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
