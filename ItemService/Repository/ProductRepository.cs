using Contracts.IRepository;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Repository
{
    /// <summary>
    /// Handles all the query operations for the Product table
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly RepositoryContext context;
        private readonly ILogger<ProductRepository> logger;

        public ProductRepository(RepositoryContext context, ILogger<ProductRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Saves all the changes made in the context to the DB
        /// </summary>
        public void SaveChanges()
        {
            logger.LogDebug("Saving all the changes made in the context to the DB");

            context.SaveChanges();
        }


        /// <summary>
        /// Checks if the product name already exists in the DB
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ProductExists(string name)
        {
            logger.LogDebug("Received request to check if a product with the name: {0} already exists for the user", name);

            return context.Product.Any(a => a.Name == name && a.IsActive == true);
        }


        /// <summary>
        /// Adds a product detail to the DB
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product)
        {
            logger.LogDebug("Received request to add a new product with the Id: " + product.Id);

            context.Product.Add(product);
        }

        /// <summary>
        ///  Get all products
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Product> GetAllProducts(string role)
        {
            logger.LogDebug("Received request to get all products");

            if (role == "Admin")
            {
                return context.Product.Where(p => p.IsActive == true).OrderBy(p => p.Name).ToList();
            }

            return context.Product.Where(p => p.IsActive == true && p.Visibility == true && p.AvailableCount > 0)
            .OrderBy(p => p.Name).ToList();
        }


        /// <summary>
        /// Get a product by product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Product GetProductById(Guid productId, string role)
        {
            logger.LogDebug("Received request to get a product at " + productId);

            if (role == "Admin")
            {
                return context.Product.Where(p => p.Id == productId && p.IsActive == true).SingleOrDefault();
            }

            return context.Product.Where(p => p.Id == productId && p.IsActive == true && p.Visibility == true && p.AvailableCount > 0).SingleOrDefault();
        }

        /// <summary>
        /// Checks if the product exists for the given product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool VerifyProductId(Guid productId)
        {
            logger.LogDebug("Received request to verify a product at " + productId);

            return context.Product.Any(p => p.Id == productId && p.IsActive == true && p.Visibility == true && p.AvailableCount > 0);
        }

        public List<Product> GetAllProducts()
        {
            logger.LogDebug("Received request to get all products");
            return context.Product.Where(p => p.IsActive == true && p.Visibility == true && p.AvailableCount > 0)
            .OrderBy(p => p.Name).ToList();
        }
    }
}
