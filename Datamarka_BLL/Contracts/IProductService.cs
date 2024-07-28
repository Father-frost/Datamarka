using Datamarka_DomainModel.Models.ECommerce;
using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_BLL.Contracts
{
    public interface IProductService : IService
    {
        public List<Product> GetProducts();

        public Product GetProductById(long? productId);
        public Task<Product> CreateProduct(Product product);

        public Task WriteProduct(Product productToWrite);

        public Task DeleteProduct(long productId);
    }
}
