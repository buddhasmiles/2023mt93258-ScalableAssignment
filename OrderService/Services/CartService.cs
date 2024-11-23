using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly IMapper mapper;
        private readonly ICommonService commonService;
        private readonly ICartRepository cartRepository;
        private readonly ILogger<CartService> logger;

        public CartService(IMapper mapper, ICommonService commonService, ICartRepository cartRepository,
                           ILogger<CartService> logger)
        {
            this.mapper = mapper;
            this.commonService = commonService;
            this.cartRepository = cartRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Adds a list of product to cart with its quantity
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addToCart"></param>
        public void AddProductToCart(AddToCartDto addToCart)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to add product(s) to the user with the Id: " + userId);

            commonService.VerifyUserId();

            List<Guid> productIds = addToCart.Products.Select(p => p.ProductId).ToList();

            commonService.VerifyProductIdAsList(productIds);

            List<Cart> newProducts = mapper.Map<List<Cart>>(addToCart.Products);
            newProducts = newProducts.Select(np => { np.UserId = userId; return np; }).ToList();

            cartRepository.AddProductToCart(newProducts);
            cartRepository.SaveChanges();

            logger.LogDebug("Added product(s) to the user with the Id: " + userId);
        }


        /// <summary>
        /// Gets all products from the cart with its quantity
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CartResponseDto> GetAllProductsFromCartByUserId()
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to get all product(s) in the cart of a user with the Id: " + userId);

            commonService.VerifyUserId();

            List<Cart> productsInCart = cartRepository.GetAllProducts(userId);

            logger.LogDebug("Fetched all the products from the cart of a user with the Id: " + userId);

            return mapper.Map<List<CartResponseDto>>(productsInCart);
        }


        /// <summary>
        /// Updates a cart of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updatedCart"></param>
        public void UpdateCartByUserId(AddToCartUpdateDto updatedCart)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to update the cart of a user with the Id: " + userId);

            commonService.VerifyUserId();

            List<Cart> productsInCart = cartRepository.GetAllProducts(userId);

            if (productsInCart.Count == 0)
            {
                logger.LogError("No product has been added to the cart for the user Id: " + userId);

                throw new BadRequestException("No product has been added to the cart");
            }

            IEnumerable<Guid> currentProductIds = productsInCart.Select(i => i.ProductId); 
            IEnumerable<Guid> newProductIds = updatedCart.Products.Select(i => i.ProductId);

            // Deletes the cart product items that are present in the DB but not in the updated wishlist

            productsInCart.Where(i => !newProductIds.Contains(i.ProductId)).ToList().ForEach(p => p.IsActive = false);

            // Updates the quantity (if there is a change) for the items that are present in both DB and updated wish list

            productsInCart.Where(c => updatedCart.Products.Any(n => n.ProductId == c.ProductId && n.Quantity != c.Quantity))
            .Join(updatedCart.Products, c => c.ProductId, n => n.ProductId, (c, n) => { c.Quantity = n.Quantity; 
            c.DateUpdated = DateTime.Now; return c; }).ToList();

            // Adds new product that are not present in the DB (cart)

            List<CartProductUpdateDto> newProductItemsToAdd = updatedCart.Products.Where(i => !currentProductIds.Contains(i.ProductId)).ToList();

            if (newProductItemsToAdd.Count != 0)
            {
                List<Guid> productIds = newProductItemsToAdd.Select(ni => ni.ProductId).ToList();

                commonService.VerifyProductIdAsList(productIds);

                List<Cart> cartItems = mapper.Map<List<Cart>>(newProductItemsToAdd);
                cartItems = cartItems.Select(ci => { ci.UserId = userId; return ci; }).ToList();

                cartRepository.AddProductToCart(cartItems);
            }

            cartRepository.SaveChanges();

            logger.LogDebug("Updated the cart of a user with the Id: " + userId);
        }

    }
}
