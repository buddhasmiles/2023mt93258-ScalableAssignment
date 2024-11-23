using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<ProductService> logger;

        public ProductService(IMapper mapper, IProductRepository productRepository, 
                              IHttpContextAccessor httpContextAccessor, ILogger<ProductService> logger)
        {
            this.mapper = mapper;
            this.productRepository = productRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        /// <summary>
        /// Gets only the role of a user from claims
        /// </summary>
        /// <returns></returns>
        public string GetRoleFromClaims()
        {
            return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }

        /// <summary>
        /// Adds a product to the DB
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns></returns>
        /// <exception cref="ConflictException"></exception>
        public IdentityResponseDto CreateProductDetail(CreateProductDto newProduct)
        {
            logger.LogDebug("Received request to add product");

            if (productRepository.ProductExists(newProduct.Name))
            {
                logger.LogError("Product already exists with the name: ", newProduct.Name);

                throw new ConflictException("Product name already exists");
            }

            Product product = mapper.Map<Product>(newProduct);

            productRepository.AddProduct(product);
            productRepository.SaveChanges();

            logger.LogDebug("New product created with the Id: " + product.Id);

            return new IdentityResponseDto { Id = product.Id };
        }


        /// <summary>
        /// Get all products and groups them by their category
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductAllResponseDto> GetAllProducts()
        {
            logger.LogDebug("Received request to get all product details");

            List<Product> products = productRepository.GetAllProducts();

            IEnumerable<ProductResponseDto> mappedProducts = products.Select(p => mapper.Map<ProductResponseDto>(p));

            IEnumerable<ProductAllResponseDto> groupedProducts = mappedProducts.GroupBy(p => p.Category)
            .Select(g => new ProductAllResponseDto
            {
                Category = g.Key,
                Products = g.ToList()
            });

            return groupedProducts;
        }


        /// <summary>
        /// Gets a product detail by its product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public ProductResponseDto GetProductByProductId(Guid productId)
        {
            logger.LogDebug("Received request to get product details for ID: " + productId);

            Product product = productRepository.GetProductById(productId, GetRoleFromClaims());

            if (product == null)
            {
                logger.LogError("No product has been found for the Id: " + productId);

                throw new NotFoundException("No product has been found for the given id");
            }

            logger.LogDebug("Product at {0} has been fetched and returned", productId);

            return mapper.Map<ProductResponseDto>(product);
        }
   
        
        /// <summary>
        /// Updates a product by its id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updatedProduct"></param>
        public void UpdateProductByProductId(Guid productId, ProductUpdateDto updatedProduct)
        {
            logger.LogDebug("Received request to update product details for the ID: " + productId);

            Product productInDB = productRepository.GetProductById(productId, "Admin");

            if (productInDB == null)
            {
                logger.LogError("No product has been found with the Id: " + productId);

                throw new NotFoundException("No product has been found");
            }

            if (updatedProduct.Name != null)
            {
                if (productRepository.ProductExists(updatedProduct.Name))
                {
                    logger.LogError("Product already exists with the name " + updatedProduct.Name);

                    throw new ConflictException("Product name already exists");
                }
            }

            var properties = typeof(ProductUpdateDto).GetProperties();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(updatedProduct);

                if (propertyValue != null)
                {
                    productInDB.GetType().GetProperty(propertyName).SetValue(productInDB, propertyName == "Image" ? Convert.FromBase64String(updatedProduct.Image) : propertyValue);
                }
            }

            productInDB.DateUpdated = DateTime.Now;
            productRepository.SaveChanges();

            logger.LogDebug("Product details has been updated for the ID: " + productId);
        }


        /// <summary>
        /// Deletes a product by its id
        /// </summary>
        /// <param name="productId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void DeleteProductByProductId(Guid productId)
        {
            logger.LogDebug("Received request to a product with the ID: " + productId);

            Product productInDB = productRepository.GetProductById(productId, "Admin");

            if (productInDB == null)
            {
                logger.LogError("No product has been found with the Id: " + productId);

                throw new NotFoundException("No product has been found");
            }

            productInDB.IsActive = false;
            productRepository.SaveChanges();

            logger.LogDebug("Product has been deleted with the Id: " + productId);
        }

        // The below methods are specific to inter-service communication

        /// <summary>
        /// Checks if the product exists and returns the product id that is not found
        /// </summary>
        /// <param name="productId"></param>
        /// <exception cref="NotFoundException"></exception>
        public Guid ProductIdsExists(List<Guid> productIds)
        {
            logger.LogDebug("Received request to verify a list of product Ids");

            foreach (Guid productId in productIds)
            {
                if (!productRepository.VerifyProductId(productId))
                {
                    logger.LogError("No product has been found with the Id: " + productId);

                    return productId;
                }
            }

            logger.LogDebug("Successfully verified all the product Ids");

            return Guid.Empty;
        }
    }
}



