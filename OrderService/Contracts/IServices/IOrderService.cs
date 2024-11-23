using Entities.Dtos;

namespace Contracts.IServices
{
    public interface IOrderService
    {
        /// <summary>
        /// Check out products from cart and places an order
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public IdentityResponseDto CreateOrder(CheckOutFromCartDto checkOut);

        /// <summary>
        /// Gets order details of a user by order id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public OrderResponseDto GetOrderDetailsByOrderId(Guid orderId);

        /// <summary>
        /// Gets entire order history of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<OrderResponseDto> GetAllOrderDetails();
    }
}
