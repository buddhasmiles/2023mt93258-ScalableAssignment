using Entities.Dtos;

namespace Contracts.IServices
{
    public interface IWishListService
    {
        /// <summary>
        /// Creates a wish list grouped by a wish list name
        /// </summary>
        /// <param name="newWishList"></param>
        /// <returns></returns>
        public IdentityResponseDto CreateWishList(WishListCreateDto newWishList);

        /// <summary>
        /// Gets a wish list information for the given wish list id
        /// </summary>
        /// <param name="wishListId"></param>
        /// <returns></returns>
        public WishListResponseDto GetWishListByWishListId(Guid wishListId);

        /// <summary>
        /// Updates a wish list by flush & fill 
        /// </summary>
        /// <param name="wishListId"></param>
        /// <param name="updatedWishList"></param>
        public void UpdateWishListByWishListId(Guid wishListId, WishListUpdateDto updatedWishList);

        /// <summary>
        /// Gets all the wish list information of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<WishListResponseDto> GetAllWishListByUserId();

        /// <summary>
        /// Soft deletes an entire wish list by its id
        /// </summary>
        /// <param name="wishListId"></param>
        public void DeleteWishListByWishListId(Guid wishListId);
    }
}
