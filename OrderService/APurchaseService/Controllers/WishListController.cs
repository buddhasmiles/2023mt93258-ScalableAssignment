using Contracts.IServices;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderService.Controllers
{
    [Route("api/wish-list")]
    [ApiController]
    [Authorize]
    public class WishListController : Controller
    {
        private readonly IWishListService wishListService;
        private readonly ILogger<WishListController> logger;

        public WishListController(IWishListService wishListService, ILogger<WishListController> logger)
        {
            this.wishListService = wishListService;
            this.logger = logger;
        }

        /// <summary>
        /// Creates a wish list for the user
        /// </summary>
        /// <param name="newWishList"></param>
        /// <response code="201">Wish list created successfully</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">Product may not exists/may be out of stock</response>
        /// <response code="409">Wish list name already exists</response>
        [HttpPost]
        [SwaggerResponse(statusCode: 201, description: "Wish list created successfully", type: typeof(IdentityResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Product may not exists/may be out of stock", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "Wish list name already exists", type: typeof(ErrorResponseDto))]
        public IActionResult CreateWishList([FromBody] WishListCreateDto newWishList)
        {
            logger.LogInformation("Received request to create a wish list for a user");

            IdentityResponseDto createdWishListId = wishListService.CreateWishList(newWishList);

            logger.LogInformation("Successfully created a wish list for the user with the Id: " + createdWishListId.Id);

            return StatusCode(StatusCodes.Status201Created, createdWishListId);
        }

        /// <summary>
        /// Get a wish list by its id
        /// </summary>
        /// <param name="wishListId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No wish list exists for the given id</response>
        [HttpGet("{wishListId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(WishListResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No wish list exists for the given id", type: typeof(ErrorResponseDto))]
        public IActionResult GetWishListById([FromRoute] Guid wishListId)
        {
            logger.LogInformation("Received request to get a wish list at " + wishListId);

            WishListResponseDto wishListAtId = wishListService.GetWishListByWishListId(wishListId);

            logger.LogInformation("Wish list at {0} has been fetched and returned ", wishListId);

            return StatusCode(StatusCodes.Status200OK, wishListAtId);
        }

        /// <summary>
        /// Get all wish list details of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="204">No content has been found</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpGet]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(List<WishListResponseDto>))]
        [SwaggerResponse(statusCode: 204, description: "No content has been found")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult GetAllWishListByUserId()
        {
            logger.LogInformation("Received request to get all wish lists of a user");

            List<WishListResponseDto> wishLists = wishListService.GetAllWishListByUserId();

            if (wishLists.Count == 0)
            {
                logger.LogInformation("No wish list has been found for the user");

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("All wish lists has been fetched and returned for the user");

            return StatusCode(StatusCodes.Status200OK, wishLists);
        }

        /// <summary>
        /// Update a wish list 
        /// </summary>
        /// <param name="wishListId"></param>
        /// <param name="updatedWishList"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">Product may not exists/may be out of stock</response>
        /// <response code="409">Wish list name already exists</response>
        [HttpPut("{wishListId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "Product may not exists/may be out of stock", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "Wish list name already exists", type: typeof(ErrorResponseDto))]
        public IActionResult UpdateWishListById([FromRoute] Guid wishListId, [FromBody] WishListUpdateDto updatedWishList)
        {
            logger.LogInformation("Received request to update a wish list at " + wishListId);

            wishListService.UpdateWishListByWishListId(wishListId, updatedWishList);

            logger.LogInformation("Wish list at {0} has been successfully updated ", wishListId);

            return StatusCode(StatusCodes.Status200OK);
        }

        /// <summary>
        /// Deletes an entire wish list by its id
        /// </summary>
        /// <param name="wishListId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="404">No wish list has been found for the given id</response>
        [HttpDelete("{wishListId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 404, description: "No wish list exists for the given id", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult DeleteWishListById([FromRoute] Guid wishListId)
        {
            logger.LogInformation("Received request to delete an entire wish list with the Id: " + wishListId);

            wishListService.DeleteWishListByWishListId(wishListId);

            logger.LogInformation("Wish list at {0} has been successfully deleted: ", wishListId);

            return StatusCode(StatusCodes.Status200OK);
        }

    }
}
