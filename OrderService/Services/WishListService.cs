using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class WishListService : IWishListService
    {
        private readonly IMapper mapper;
        private readonly IWishListRepository wishListRepository;
        private readonly ICommonService commonService;
        private readonly ILogger<WishListService> logger;

        public WishListService(IMapper mapper, IWishListRepository wishListRepository,
                               ICommonService commonService, ILogger<WishListService> logger)
        {
            this.mapper = mapper;
            this.wishListRepository = wishListRepository;
            this.commonService = commonService;
            this.logger = logger;
        }

        /// <summary>
        /// Creates a wish list grouped by a wish list name
        /// </summary>
        /// <param name="newWishList"></param>
        /// <returns></returns>
        public IdentityResponseDto CreateWishList(WishListCreateDto newWishList)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to create a wish list for the user with the Id: " + userId);

            commonService.VerifyUserId();

            if (wishListRepository.WishListNameExists(userId, newWishList.Name))
            {
                logger.LogError("Wish list name already exists for the user Id: " + userId);

                throw new ConflictException("Wish List name already exists");
            }

            List<Guid> newProductIds = newWishList.WishListItem.Select(i => i.ProductId).ToList();

            commonService.VerifyProductIdAsList(newProductIds);

            WishList wishList = mapper.Map<WishList>(newWishList);
            wishList.UserId = userId;

            wishListRepository.AddWishList(wishList);
            wishListRepository.SaveChanges();

            logger.LogDebug("New wish list has been created with the Id: {0} for the user with the Id: {1} " + wishList.Id, userId);

            return new IdentityResponseDto { Id = wishList.Id };
        }

        /// <summary>
        /// Gets a wish list information for the given wish list id
        /// </summary>
        /// <param name="wishListId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public WishListResponseDto GetWishListByWishListId(Guid wishListId)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to get a wish list at " + wishListId);

            commonService.VerifyUserId();

            WishList wishListInDB = wishListRepository.GetWishListByWishListId(userId, wishListId);

            if (wishListInDB == null)
            {
                logger.LogError("No wish list has been found at " + wishListId);

                throw new NotFoundException("No wish list exists for the given id");
            }

            logger.LogDebug("Fetched and returned wish list at " + wishListId);

            return mapper.Map<WishListResponseDto>(wishListInDB);
        }

        /// <summary>
        /// Gets all the wish list information of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public List<WishListResponseDto> GetAllWishListByUserId()
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to get all wish lists for the user id: " + userId);

            commonService.VerifyUserId();

            List<WishList> wishLists = wishListRepository.GetAllWishList(userId);

            logger.LogDebug("Fetched and returned all wish lists for the user Id: " + userId);

            return mapper.Map<List<WishListResponseDto>>(wishLists);
        }

        /// <summary>
        /// Updates a wish list 
        /// </summary>
        /// <param name="wishListId"></param>
        /// <param name="updatedWishList"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ConflictException"></exception>
        
        public void UpdateWishListByWishListId(Guid wishListId, WishListUpdateDto updatedWishList)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to update a wish list at {0} for the user Id: {1}", wishListId, userId);

            commonService.VerifyUserId();

            WishList currentWishListInDb = wishListRepository.GetWishListByWishListId(userId, wishListId);

            if (currentWishListInDb == null)
            {
                logger.LogError("No wish list has been found at " + wishListId);

                throw new NotFoundException("No wish list has been found for the given id");
            }

            if (updatedWishList.Name != string.Empty)
            {
                if (wishListRepository.WishListNameExists(userId, updatedWishList.Name))
                {
                    logger.LogError("Wish list name already exists");

                    throw new ConflictException("Wish List name already exists");
                }

                currentWishListInDb.Name = updatedWishList.Name;
            }

            IEnumerable<Guid> currentProductIds = currentWishListInDb.WishListItem.Select(i => i.ProductId); 
            IEnumerable<Guid> newProductIds = updatedWishList.WishListItem.Select(i => i.ProductId); 


            // Deletes the wish list product items that are present in the DB but not in the updated wishlist

            currentWishListInDb.WishListItem.Where(i => !newProductIds.Contains(i.ProductId)).ToList().ForEach(i => i.IsActive = false);

            // Add only the items that are not present in the DB

            List<WishListItemUpdateDto> newWishListItemsToAdd = updatedWishList.WishListItem.Where(i => !currentProductIds.Contains(i.ProductId)).ToList();

            if (newWishListItemsToAdd.Count != 0)
            {
                List<Guid> productIdsInNewWishListItems = newWishListItemsToAdd.Select(ni => ni.ProductId).ToList();

                commonService.VerifyProductIdAsList(productIdsInNewWishListItems);

                List<WishListItem> wishListItemsToAdd = mapper.Map<List<WishListItem>>(newWishListItemsToAdd);
                wishListItemsToAdd = wishListItemsToAdd.Select(wi => { wi.WishListId = currentWishListInDb.Id; return wi; }).ToList();

                wishListRepository.AddWishListItem(wishListItemsToAdd);
            }

            currentWishListInDb.DateUpdated = DateTime.Now;

            wishListRepository.SaveChanges();

            logger.LogDebug("Received request to update wish list at {0} for the user Id: {1}", wishListId, userId);
        }

        /// <summary>
        /// Deletes an entire wish list by its id
        /// </summary>
        /// <param name="wishListId"></param>
        /// <exception cref="NotFoundException"></exception>

        public void DeleteWishListByWishListId(Guid wishListId)
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to delete a wish list at {0} for the user Id: {1}", wishListId, userId);

            commonService.VerifyUserId();

            WishList wishListInDb = wishListRepository.GetWishListByWishListId(userId, wishListId);

            if (wishListInDb == null)
            {
                logger.LogError("No wish list has been found at " + wishListId);

                throw new NotFoundException("No wish list has been found for the given id");
            }

            wishListInDb.WishListItem.ForEach(i => i.IsActive = false);
            wishListInDb.IsActive = false;

            wishListRepository.SaveChanges();

            logger.LogDebug("Deleted an entire wish list at {0} for the user Id: {1}", wishListId, userId);
        }
    }
}
    