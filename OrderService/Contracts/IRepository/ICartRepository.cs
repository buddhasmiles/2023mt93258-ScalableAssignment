using Entities.Models;

namespace Contracts.IRepository
{
    public interface ICartRepository
    {
        /// <summary>
        /// Saves all the changes in the context to the DB
        /// </summary>
        public void SaveChanges();

        /// <summary>
        /// Adds a list of new products to the cart
        /// </summary>
        /// <param name="newProduct"></param>
        public void AddProductToCart(List<Cart> newProduct);

        /// <summary>
        /// Gets all products from cart of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Cart> GetAllProducts(Guid userId);
    }
}
