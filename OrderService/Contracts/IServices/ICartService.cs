using Entities.Dtos;

namespace Contracts.IServices
{
    public interface ICartService
    {
        /// <summary>
        /// Adds a list of product to cart with its quantity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addToCart"></param>
        public void AddProductToCart(AddToCartDto addToCart);

        /// <summary>
        /// Gets all products from the cart with its quantity
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CartResponseDto> GetAllProductsFromCartByUserId();

        /// <summary>
        /// Updates a cart by flush and fill
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updatedCart"></param>
        public void UpdateCartByUserId(AddToCartUpdateDto updatedCart);
    }
}
