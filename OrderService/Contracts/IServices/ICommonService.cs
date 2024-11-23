namespace Contracts.IServices
{
    public interface ICommonService
    {
        /// <summary>
        /// Gets the user id from claims
        /// </summary>
        /// <returns></returns>
        public Guid GetUserIdFromClaims();

        /// <summary>
        /// Checks if the user id exists in the user service DB
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyUserId();

        /// <summary>
        /// Checks if the product ids exists in product service DB
        /// </summary>
        /// <param name="productIds"></param>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyProductIdAsList(List<Guid> productIds);

        /// <summary>
        /// Checks if the address id exists for the user id in the user service DB
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyAddressId(Guid addressId);

        /// <summary>
        /// Checks if the payment id exists for the user id in the user service DB
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyPaymentId(Guid? paymentId);
    }
}
