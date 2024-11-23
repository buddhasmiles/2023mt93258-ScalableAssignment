using Contracts.IServices;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderService.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly ILogger<CartController> logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            this.cartService = cartService;
            this.logger = logger;
        }

        /// <summary>
        /// Adds a new product to the cart with its quantity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newProduct"></param>
        /// <response code="201">Product added successfully</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">Product may not exists/may be out of stock</response>
        [HttpPost]
        [SwaggerResponse(statusCode: 201, description: "Product added successfully", type: typeof(IdentityResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Product may not exists/may be out of stock", type: typeof(ErrorResponseDto))]
        public IActionResult AddProductToCart([FromBody] AddToCartDto newProduct)
        {
            logger.LogInformation("Received request to product(s) to the cart");

            cartService.AddProductToCart(newProduct);

            logger.LogInformation("Added product(s) to the cart");

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Gets complete list of product items in a cart
        /// </summary>
        /// <response code="200">Successfull operation</response>
        /// <response code="204">No content has been found</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpGet]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(List<CartResponseDto>))]
        [SwaggerResponse(statusCode: 204, description: "No content has been found")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult GetCartItemsByUserId()
        {
            logger.LogInformation("Received request get all products in cart of a user");

            List<CartResponseDto> products = cartService.GetAllProductsFromCartByUserId();

            if (products.Count == 0)
            {
                logger.LogInformation("No product has been found in the user's cart");

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("Returned all products in cart of the user");

            return StatusCode(StatusCodes.Status200OK, products);
        }

        /// <summary>
        /// Update a cart 
        /// </summary>
        /// <param name="wishListId"></param>
        /// <param name="updatedWishList"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="400">No product has been added to the cart</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpPut]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "No product has been added to the cart", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult UpdateCartByUserId([FromBody] AddToCartUpdateDto updatedCart)
        {
            logger.LogInformation("Received request to update the cart of a user");

            cartService.UpdateCartByUserId(updatedCart);

            logger.LogInformation("Cart has been updated for the user");

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
