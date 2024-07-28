using EllipticCurve.Utils;
using Datamarka_BLL.Contracts;
using Datamarka_BLL.Contracts.Identity;
using Datamarka_DAL;
using Datamarka_DomainModel.Models.ECommerce;
using Datamarka_DomainModel.Models.Identity;
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
            _userService = userService;
        }

        public Task<List<OrderBriefModel>> FetchOrders(long skip, long take, string? searchString, OrderStatusEnum? status)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var query = repo.AsReadOnlyQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                query = from order in query
                        where order.User.Id == searchString
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
                                     User = order.User,
                                     Product = order.Product,
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
                        where order.Product.Id == 1
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
                                     User = order.User,
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

            var order = repo.AsReadOnlyQueryable()
                .FirstOrDefault(ord => ord.Id == orderId);

            return order;
        }

        public async Task<Order> CreateOrder(OrderBriefModel order)
        {
            var repo = _unitOfWork.GetRepository<Order>();



            var newDbOrder = new Order
            {
                User = order.User,
                OrderCode = order.OrderCode,
                Product = order.Product,
                ProdDate = order.ProdDate,
                WarrantDate = order.WarrantDate,
                Status = order.Status,
            };

            var trackedOrder = repo.Create(newDbOrder);

            await _unitOfWork.SaveChangesAsync();

            return trackedOrder;
        }

        public async Task WriteOrder(OrderBriefModel orderToWrite)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var user = await _userService.GetUserById(orderToWrite.UserId);

			var newOrder = new Order
			{
                User = orderToWrite.User,
                OrderCode = orderToWrite.OrderCode,
                Product = orderToWrite.Product,
                ProdDate = orderToWrite.ProdDate,
                WarrantDate = orderToWrite.WarrantDate,
                Status = orderToWrite.Status,
            };

			repo.InsertOrUpdate(
            order => order.Id == orderToWrite.Id,
            newOrder
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
