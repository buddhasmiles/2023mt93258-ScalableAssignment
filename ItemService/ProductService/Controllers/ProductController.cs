using AProductService.Helpers;
using Contracts.IServices;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AProductService.Controllers
{
    /// <summary>
    /// Handles all the operations for the product
    /// </summary>
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductController> logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            this.productService = productService;
            this.logger = logger;
        }

        /// <summary>
        /// Admins can add a product information
        /// </summary>
        /// <param name="newProduct"></param>
        /// <response code="201">Product added successfully</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="403">Customers don't have access to this resource</response>
        /// <response code="409">Product name already exists</response>
        [HttpPost]
        //[AdminAccess]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 201, description: "Product added successfully", type: typeof(IdentityResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 403, description: "Customers don't have access to this resource", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "Product name already exists", type: typeof(ErrorResponseDto))]
        public IActionResult CreateProduct([FromBody] CreateProductDto newProduct)
        {
            logger.LogInformation("Received request to add new product");

            IdentityResponseDto createdProductId = productService.CreateProductDetail(newProduct);

            logger.LogInformation("Added a new product with the Id: " + createdProductId.Id);

            return StatusCode(StatusCodes.Status201Created, createdProductId);
        }


        /// <summary>
        /// Gets all products and groups them by their category
        /// </summary>
        /// <response code="200">Successfull operation</response>
        /// <response code="204">No content has been found</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(IEnumerable<ProductAllResponseDto>))]
        [SwaggerResponse(statusCode: 204, description: "No content has been found")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        public IActionResult GetAllProducts()
        {
            logger.LogInformation("Received request to get all product details");

            IEnumerable<ProductAllResponseDto> groupedProducts = productService.GetAllProducts();

            if (!groupedProducts.Any())
            {
                logger.LogInformation("No product records found. Returning 0 records.");

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("Returning all product records");

            return StatusCode(StatusCodes.Status200OK, groupedProducts);
        }


        /// <summary>
        /// Gets a product detail by its product id
        /// </summary>
        /// <param name="productId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No product has been found for the given id</response>
        [HttpGet("{productId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(ProductResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No product has been found for the given id", type: typeof(ErrorResponseDto))]
        public IActionResult GetProductById([FromRoute] Guid productId)
        {
            logger.LogInformation("Received request to get product information for the Id: " + productId);

            ProductResponseDto productAtId = productService.GetProductByProductId(productId);

            logger.LogInformation("Returning the complete product information with the Id: " + productId);

            return StatusCode(StatusCodes.Status200OK, productAtId);
        }

        /// <summary>
        /// Updates all/some properties of the existing product by id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updatedProduct"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="403">Customers don't have access to this resource</response>
        /// <response code="404">No product has been found for the given id</response>
        /// <response code="409">Product name already exists</response>
        [HttpPut("{productId:guid}")]
        [AdminAccess]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 403, description: "Customers don't have access to this resource", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No product has been found for the given id", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "Product name already exists", type: typeof(ErrorResponseDto))]
        public IActionResult UpdateProduct([FromRoute] Guid productId, [FromBody] ProductUpdateDto updatedProduct)
        {
            logger.LogInformation("Received request to update product information for the Id: " + productId);

            productService.UpdateProductByProductId(productId, updatedProduct);

            logger.LogInformation("Updated the product information with the Id: " + productId);

            return StatusCode(StatusCodes.Status200OK);
        }

        /// <summary>
        /// Deletes a product by its id
        /// </summary>
        /// <param name="productId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="403">Customers don't have access to this resource</response>
        /// <response code="404">No product has been found for the given id</response>
        [HttpDelete("{productId:guid}")]
        [AdminAccess]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 403, description: "Customers don't have access to this resource", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No product has been found for the given id", type: typeof(ErrorResponseDto))]
        public IActionResult DeleteProduct([FromRoute] Guid productId)
        {
            logger.LogInformation("Received request to delete a product with the Id: " + productId);

            productService.DeleteProductByProductId(productId);

            logger.LogInformation("Deleted a product with the Id: " + productId);

            return StatusCode(StatusCodes.Status200OK);
        }

        // The below methods are specific to inter-service communication

        /// <summary>
        /// Checks if a product exists for the given id
        /// </summary>
        /// <param name="productId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="404">No product has been found for the given id</response>
        [HttpPost("verify")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 404, description: "No product has been found for the given id", type: typeof(ErrorResponseDto))]
        public IActionResult VerifyProductIdAsList([FromBody] List<Guid> productIds)
        {
            logger.LogInformation("Received request to verify a list of product Ids");

            Guid notFoundProductId = productService.ProductIdsExists(productIds);

            if (notFoundProductId != Guid.Empty)
            {
                logger.LogError("No product has been found with {0} during verification", notFoundProductId);
                return StatusCode(StatusCodes.Status404NotFound, notFoundProductId);
            }

            logger.LogInformation("All the products Ids are verified");

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
