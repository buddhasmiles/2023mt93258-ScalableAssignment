using Contracts.IServices;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderService.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly ILogger<OrderController> logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            this.orderService = orderService;
            this.logger = logger;
        }

        /// <summary>
        /// Places an order by adding the products from cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="checkOut"></param>
        /// <response code="201">Order placed successfully</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="400">No product has been added to the cart</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No address has been found for the given id</response>
        /// <response code="404">No payment information has been found</response>
        [HttpPost]
        [SwaggerResponse(statusCode: 201, description: "Order placed successfully", type: typeof(IdentityResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "No product has been added to the cart", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No address has been found for the given id", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No payment information has been found for the given id", type: typeof(ErrorResponseDto))]
        public IActionResult CreateOrder([FromBody] CheckOutFromCartDto checkOut)
        {
            logger.LogInformation("Received request to place an order for a user");

            IdentityResponseDto orderId = orderService.CreateOrder(checkOut);

            if (orderId.Id == Guid.Empty)
            {
                logger.LogInformation("Order can't be placed as the user does't have any items added to the cart");

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("Successfully placed an order for the user with the order Id: " + orderId);

            return StatusCode(StatusCodes.Status201Created, orderId);
        }

        /// <summary>
        /// Gets an order detail by its id
        /// </summary>
        /// <param name="orderId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No order exists for the given id</response>
        [HttpGet("{orderId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(OrderResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No order exists for the given id", type: typeof(ErrorResponseDto))]
        public IActionResult GetOrderById([FromRoute] Guid orderId)
        {
            logger.LogInformation("Received request to get order information at " + orderId);

            OrderResponseDto orderAtId = orderService.GetOrderDetailsByOrderId(orderId);

            logger.LogInformation("Order information at {0} has been fetched and returned", orderId);

            return StatusCode(StatusCodes.Status200OK, orderAtId);
        }

        /// <summary>
        /// Gets entire order history of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="204">No content has been found</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpGet]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(List<OrderResponseDto>))]
        [SwaggerResponse(statusCode: 204, description: "No content has been found")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult GetAllOrderDetails()
        {
            logger.LogInformation("Received request to get complete order history of a user");

            List<OrderResponseDto> orders = orderService.GetAllOrderDetails();

            if (orders.Count == 0)
            {
                logger.LogInformation("No order history has been found for the user");

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("Complete order history has been fetched and returned for the user");

            return StatusCode(StatusCodes.Status200OK, orders);
        }

    }
}
