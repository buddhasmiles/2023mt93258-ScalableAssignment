using Entities.Dtos;
using Entities.Models;

namespace Contracts.IServices
{
    public interface IProductService
    {
        /// <summary>
        /// Adds a product to the DB
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns></returns>
        public IdentityResponseDto CreateProductDetail(CreateProductDto newProduct);

        /// <summary>
        /// Get all products and groups them by their category
        /// </summary>
        /// <returns></returns>

        public IEnumerable<ProductAllResponseDto> GetAllProducts();

        /// <summary>
        /// Gets a product detail by its product id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ProductResponseDto GetProductByProductId(Guid productId);

        /// <summary>
        /// Updates a product by its id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updatedProduct"></param>
        public void UpdateProductByProductId(Guid productId, ProductUpdateDto updatedProduct);

        /// <summary>
        /// Deletes a product by its id
        /// </summary>
        /// <param name="productId"></param>
        public void DeleteProductByProductId(Guid productId);

        /// <summary>
        /// Checks if the product exists and returns the product id that is not found
        /// </summary>
        /// <param name="productId"></param>
        public Guid ProductIdsExists(List<Guid> productIds);
    }
}
