using Contracts.IRepository;
using Entities.Models;

namespace Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly RepositoryContext context;

        public CartRepository(RepositoryContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Saves all the changes in the context to the DB
        /// </summary>
        public void SaveChanges()
        {
            context.SaveChanges();
        }

        /// <summary>
        /// Adds a list of new products to the cart
        /// </summary>
        /// <param name="newProduct"></param>
        public void AddProductToCart(List<Cart> newProduct)
        {
            context.Cart.AddRange(newProduct);
        }

        /// <summary>
        /// Gets all products from cart of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Cart> GetAllProducts(Guid userId)
        {
            return context.Cart.Where(c => c.UserId == userId && c.IsActive == true).ToList();
        }
    }
}
