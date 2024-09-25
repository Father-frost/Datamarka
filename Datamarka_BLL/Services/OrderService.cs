using Datamarka_BLL.Contracts;
using Datamarka_BLL.Contracts.Identity;
using Datamarka_DAL;
using Datamarka_DomainModel.Models.ECommerce;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Datamarka_BLL.Services.Identity
{
    internal class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<OrderBriefModel>> FetchOrders(long skip, long take, string? searchString, OrderStatusEnum? status)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var query = repo.AsReadOnlyQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                query = from order in query
                        where order.UserId.ToString() == searchString
                        //|| order.EmployeeId == searchString
                        select order;
            }

            if (status != null)
            {
                query = from order in query
                        where order.Status == status
                        select order;
            }

            var projectedQuery = from order in query
                                 select new OrderBriefModel
                                 {
                                     Id = order.Id,
                                     UserId = order.UserId,
                                     ProductId = order.ProductId,
									 Product = order.Product,
									 Batch = order.Batch,
                                     ProdDate = order.ProdDate,
                                     WarrantDate = order.WarrantDate,
                                     Status = order.Status,
                                 };

            return projectedQuery.Skip((int)skip).Take((int)take).ToListAsync();
        }

        public Task<List<OrderBriefModel>> FetchOrdersForEmployee(long skip, long take, string? searchString, OrderStatusEnum? status)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var query = repo.AsReadOnlyQueryable();



            query = from order in query
                    where order.ProductId == 1
                    select order;


            if (status != null)
            {
                query = from order in query
                        where order.Status == status
                        select order;
            }

            var projectedQuery = from order in query
                                 select new OrderBriefModel
                                 {
                                     Id = order.Id,
                                     UserId = order.UserId,
                                     Batch = order.Batch,
                                     ProductId = order.ProductId,
									 Product = order.Product,
									 ProdDate = order.ProdDate,
                                     WarrantDate = order.WarrantDate,
                                     Status = order.Status,
                                 };

            return projectedQuery.Skip((int)skip).Take((int)take).ToListAsync();
        }

        public async Task<Order> GetOrderById(long? orderId)
        {
            var repo = _unitOfWork.GetRepository<Order>();
            var repo2 = _unitOfWork.GetRepository<Product>();

            var order = await repo.AsReadOnlyQueryable()
                .FirstOrDefaultAsync(ord => ord.Id == orderId);
            order.Product = await repo2.AsReadOnlyQueryable()
                .FirstOrDefaultAsync(prod => prod.Id == order.ProductId);

			return order;
        }

        public async Task<Order> CreateOrder(OrderBriefModel order)
        {
            var repo = _unitOfWork.GetRepository<Order>();



            var newDbOrder = new Order
            {
                UserId = order.UserId,
                Batch = order.Batch,
				ProductId = order.ProductId,
                ProdDate = order.ProdDate,
                WarrantDate = order.WarrantDate,
                Status = order.Status,
            };

            var trackedOrder = repo.Create(newDbOrder);

            await _unitOfWork.SaveChangesAsync();

            return trackedOrder;
        }

        public async Task WriteOrder(Order orderToWrite)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            repo.InsertOrUpdate(
            order => order.Id == orderToWrite.Id,
			orderToWrite
			);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrder(long orderId)
        {
            var repo = _unitOfWork.GetRepository<Order>();
            var trackedOrder = repo
                .AsQueryable()
                .First(ord => ord.Id == orderId);

            repo.Delete(trackedOrder);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
