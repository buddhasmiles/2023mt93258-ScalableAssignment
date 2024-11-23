using Contracts.IRepository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class WishListRepository : IWishListRepository
    {
        private readonly RepositoryContext context;

        public WishListRepository(RepositoryContext context)
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
        /// Checks if the given wish list name already exists for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool WishListNameExists(Guid userId, string name)
        {
            return context.WishList.Any(w => w.UserId == userId && w.Name == name && w.IsActive == true);
        }


        /// <summary>
        /// Adds a wish list record to the DB
        /// </summary>
        /// <param name="wishList"></param>
        public void AddWishList(WishList wishList)
        {
            context.WishList.Add(wishList);
        }


        /// <summary>
        /// Adds a list of wish list items to the DB
        /// </summary>
        /// <param name="wishListItem"></param>
        public void AddWishListItem(List<WishListItem> wishListItem)
        {
            context.WishListItem.AddRange(wishListItem);
        }


        /// <summary>
        /// Gets a wish list record by wish list id
        /// </summary>
        /// <param name="wishListId"></param>
        /// <returns></returns>
        public WishList GetWishListByWishListId(Guid userId, Guid wishListId)
        {
            return context.WishList.Include(wl => wl.WishListItem.Where(wli => wli.IsActive == true)).FirstOrDefault(wl => wl.UserId == userId && wl.Id == wishListId && wl.IsActive == true);
        }
        

        /// <summary>
        /// Gets all wish list of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<WishList> GetAllWishList(Guid userId)
        {
            return context.WishList.Include(wl => wl.WishListItem.Where(wli => wli.IsActive == true)).Where(wl => wl.UserId == userId && wl.IsActive == true).ToList();
        }
    }
}
