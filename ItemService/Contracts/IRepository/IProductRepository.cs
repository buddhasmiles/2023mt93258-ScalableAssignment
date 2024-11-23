using Entities.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Contracts.IRepository
{
    public interface IProductRepository
    {

        /// <summary>
        /// Saves all the changes made in the context to the DB
        /// </summary>
        public void SaveChanges();

        /// <summary>
        /// Checks if the product name already exists in the DB
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ProductExists(string name);

        /// <summary>
        /// Adds a product detail to the DB
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product);

        /// <summary>
        /// Get all products by Role
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Product> GetAllProducts(string role);

        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public List<Product> GetAllProducts();
        /// <summary>
        /// Get a product by product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Product GetProductById(Guid productId, string role);

        /// <summary>
        /// Checks if the product exists for the given product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool VerifyProductId(Guid productId);
    }
}
