using Contracts.IRepository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RepositoryContext context;

        public OrderRepository(RepositoryContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Saves all the changes in the context to the DB
        /// </summary>

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        /// <summary>
        /// Creates an order with the given address, payment method and payment id
        /// </summary>
        /// <param name="order"></param>

        public void CreateOrder(Order order)
        {
            context.Order.Add(order);
        }

        /// <summary>
        /// Adds order items mapped to an order
        /// </summary>
        /// <param name="orderItem"></param>

        public void AddOrderItem(List<OrderItem> orderItem)
        {
            context.OrderItem.AddRange(orderItem);
        }

        /// <summary>
        /// Gets complete order detail with products
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>

        public Order GetOrderByOrderId(Guid userId, Guid orderId)
        {
            return context.Order.Include(o => o.OrderItem.Where(oi => oi.IsActive == true)).FirstOrDefault(o => o.UserId == userId && o.Id == orderId && o.IsActive == true);
        }

        /// <summary>
        /// Gets entire order history of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public List<Order> GetAllOrder(Guid userId)
        {
            return context.Order.Include(o => o.OrderItem.Where(oi => oi.IsActive == true)).Where(o => o.UserId == userId && o.IsActive == true).ToList();
        }
    }
}
