using Entities.Models;

namespace Contracts.IRepository
{
    public interface IWishListRepository
    {
        /// <summary>
        /// Saves all the changes in the context to the DB
        /// </summary>
        public void SaveChanges();

        /// <summary>
        /// Checks if the given wish list name already exists for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool WishListNameExists(Guid userId, string name);

        /// <summary>
        /// Adds a wish list record to the DB
        /// </summary>
        /// <param name="wishList"></param>
        public void AddWishList(WishList wishList);

        /// <summary>
        /// Adds a list of wish list items to the DB
        /// </summary>
        /// <param name="wishListItem"></param>
        public void AddWishListItem(List<WishListItem> wishListItem);

        /// <summary>
        /// Gets a wish list record by wish list id
        /// </summary>
        /// <param name="wishListId"></param>
        /// <returns></returns>
        public WishList GetWishListByWishListId(Guid userId, Guid wishListId);

        /// <summary>
        /// Gets all wish list of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<WishList> GetAllWishList(Guid userId);
    }
}
