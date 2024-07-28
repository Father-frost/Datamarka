﻿using Datamarka_BLL.Contracts;
using Datamarka_DAL;
using Datamarka_DomainModel.Models.ECommerce;
using Datamarka_DomainModel.Models.Identity;
using System.Data;

namespace Datamarka_BLL.Services
{
	internal class ProductService : IProductService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProductService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public List<Product> GetProducts()
		{

			var repo = _unitOfWork.GetRepository<Product>();

			var query = repo.AsReadOnlyQueryable();

			query = from product in query
					select product;

			var projectedQuery = from product in query
								 select new Product
								 {
									 User = product.User,
									 Id = product.Id,
									 Post = product.Post,
									 Skills = product.Skills,
								 };

			return projectedQuery.ToList();
		}

		public Product? GetProductById(long? productId)
		{
			var repo = _unitOfWork.GetRepository<Product>();

			var product = repo.AsReadOnlyQueryable()
				.FirstOrDefault(em => em.Id == productId);

			return product;
		}

		public async Task<Product> CreateProduct(Product product)
		{
			var repo = _unitOfWork.GetRepository<Product>();

			var newDbProduct = new Product
			{
				User = product.User,
				Post = product.Post,
				Skills = product.Skills,
			};

			Product trackedProduct = repo.Create(newDbProduct);

			await _unitOfWork.SaveChangesAsync();

			return trackedProduct;
		}

		public async Task WriteEmployee(Product productToWrite)
		{
			var repo = _unitOfWork.GetRepository<Product>();

			repo.InsertOrUpdate(
			product => product.Id == productToWrite.Id,
			productToWrite
			);

			await _unitOfWork.SaveChangesAsync();
		}


		public async Task DeleteProduct(long productId)
		{
			var repo = _unitOfWork.GetRepository<Product>();
			var trackedProduct = repo
				.AsQueryable()
				.First(em => em.Id == productId);

			repo.Delete(trackedProduct);
			await _unitOfWork.SaveChangesAsync();
		}

	}
}
