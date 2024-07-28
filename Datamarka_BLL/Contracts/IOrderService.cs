using Datamarka_DomainModel.Models.ECommerce;
using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_BLL.Contracts
{
    public interface IOrderService : IService
    {
        Task<List<OrderBriefModel>> FetchOrders(long skip = 0, long take = 20, string? searchString = null, OrderStatusEnum? status = null);
        Task<List<OrderBriefModel>> FetchOrdersForEmployee(long skip = 0, long take = 20, string? searchString = null, OrderStatusEnum? status = null);

        public Task<Order> GetOrderById(long? orderId);
        public Task<Order> CreateOrder(OrderBriefModel order);

        public Task WriteOrder(OrderBriefModel orderToWrite);

        public Task DeleteOrder(long orderId);
    }
}
