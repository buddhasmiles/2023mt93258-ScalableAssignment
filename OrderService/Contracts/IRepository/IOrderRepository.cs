using Entities.Models;

namespace Contracts.IRepository
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Saves all the changes in the context to the DB
        /// </summary>
        public void SaveChanges();

        /// <summary>
        /// Creates an order with the given address, payment method and payment id
        /// </summary>
        /// <param name="order"></param>
        public void CreateOrder(Order order);

        /// <summary>
        /// Adds order items mapped to an order
        /// </summary>
        /// <param name="orderItem"></param>
        public void AddOrderItem(List<OrderItem> orderItem);

        /// <summary>
        /// Gets complete order detail with products
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order GetOrderByOrderId(Guid userId, Guid orderId);

        /// <summary>
        /// Gets entire order history of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Order> GetAllOrder(Guid userId);
    }
}
