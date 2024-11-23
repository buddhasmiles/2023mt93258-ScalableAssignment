using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper mapper;
        private readonly ICommonService commonService;
        private readonly ICartRepository cartRepository;
        private readonly IOrderRepository orderRepository;
        private readonly ILogger<OrderService> logger;

        public OrderService(IMapper mapper, ICommonService commonService, 
                            ICartRepository cartRepository, IOrderRepository orderRepository,
                            ILogger<OrderService> logger)
        {
            this.mapper = mapper;
            this.commonService = commonService;
            this.cartRepository = cartRepository;
            this.orderRepository = orderRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Check out products from cart and places an order
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>

        public IdentityResponseDto CreateOrder(CheckOutFromCartDto checkOut)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to place an order to the user with the Id: " + userId);

            commonService.VerifyUserId();

            List<Cart> productsInCart = cartRepository.GetAllProducts(userId);

            if (productsInCart.Count == 0)
            {
                logger.LogError("No product has been added to the cart");

                throw new BadRequestException("No product exists in cart");
            }

            commonService.VerifyAddressId(checkOut.AddressId);

            if (checkOut.PaymentId != null)
            {
                commonService.VerifyPaymentId(checkOut.PaymentId);
            }
            
            Order order = mapper.Map<Order>(checkOut);
            order.UserId = userId;

            orderRepository.CreateOrder(order);

            List<OrderItem> orderItem = mapper.Map<List<OrderItem>>(productsInCart);

            orderItem = orderItem.Select(oi => { oi.OrderId = order.Id; return oi; }).ToList();

            orderRepository.AddOrderItem(orderItem);
            
            // Soft deletes all the products in cart as they are now ordered

            productsInCart = productsInCart.Select(p => { p.IsActive = false; return p; }).ToList();

            orderRepository.SaveChanges();

            logger.LogDebug("Successfully placed an order with the Id: {0} for the user Id: {1}" + order.Id, userId);

            return new IdentityResponseDto { Id = order.Id };
        }

        /// <summary>
        /// Gets order details of a user by order id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>

        public OrderResponseDto GetOrderDetailsByOrderId(Guid orderId)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to get an order detail at {0} for the user with the Id: " + orderId, userId);

            commonService.VerifyUserId();

            Order orderInDB = orderRepository.GetOrderByOrderId(userId, orderId);

            if (orderInDB == null)
            {
                logger.LogError("No order exists at " + orderId);

                throw new NotFoundException("No order exists for the given id");
            }

            logger.LogDebug("Fetched and returned an order detail at {0} for the user with the Id: " + orderId, userId);

            return mapper.Map<OrderResponseDto>(orderInDB);
        }

        /// <summary>
        /// Gets entire order history of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public List<OrderResponseDto> GetAllOrderDetails()
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to get complete order history for the user Id: " + userId);

            commonService.VerifyUserId();

            List<Order> orders = orderRepository.GetAllOrder(userId);

            logger.LogDebug("Fetched and returned complete order history for the user Id: " + userId);

            return mapper.Map<List<OrderResponseDto>>(orders);
        }

    }
}
