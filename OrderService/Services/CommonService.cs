using Contracts.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class CommonService : ICommonService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpClientService client;
        private readonly ILogger<CommonService> logger;

        public CommonService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory,
                             IHttpClientService client, ILogger<CommonService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.httpClientFactory = httpClientFactory;
            this.client = client;
            this.logger = logger;
        }

        /// <summary>
        /// Gets user id from claims
        /// </summary>
        /// <returns></returns>

        public Guid GetUserIdFromClaims()
        {
            logger.LogDebug("Received request to get user id from claims");

            return new Guid(httpContextAccessor.HttpContext.User?.FindFirstValue("user_id"));
        }

        /// <summary>
        /// Checks if the user id exists in the user service DB
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyUserId()
        {
            logger.LogDebug("Received request to verify a user Id");

            Guid userId = GetUserIdFromClaims(); 

            HttpResponseMessage response = client.GetAsync($"http://localhost:5004/api/user/{userId}/verify").Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.LogError("No user has been found at " + userId);

                throw new NotFoundException("No user account has been found");
            }

            logger.LogDebug("User Id: {0} has been successfully verified", userId);
        }


        /// <summary>
        /// Checks if the product ids exists in product service DB
        /// </summary>
        /// <param name="productIds"></param>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyProductIdAsList(List<Guid> productIds)
        {
            logger.LogDebug("Received request to verify a list of product Ids");

            StringContent content = new StringContent(JsonConvert.SerializeObject(productIds), Encoding.UTF8, "application/json"); // Passing productIds in the request body

            HttpResponseMessage response = client.PostAsync($"http://localhost:5054/api/product/verify", content).Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Guid notFoundProductId = response.Content.ReadAsAsync<Guid>().Result;

                logger.LogError("No product has been founded at " +  notFoundProductId);
                
                throw new NotFoundException("Product may not exists/may be out of stock - " + notFoundProductId);
            }

            logger.LogDebug("All products has been successfully verified");
        }


        /// <summary>
        /// Checks if the address id exists for the user id in the user service DB
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyAddressId(Guid addressId)
        {
            Guid userId = GetUserIdFromClaims();

            logger.LogDebug("Received request to verify an address at {0} for the user at {1}", addressId, userId);

            HttpResponseMessage response = client.GetAsync($"http://localhost:5004/api/user/{userId}/address/{addressId}/verify").Result;

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("No address found at {0} for the user at {1}", addressId, userId);

                throw new NotFoundException("No address has been found for the given id");
            }

            logger.LogDebug("Successfully verify an address at {0} for the user at {1}", addressId, userId);
        }

        /// <summary>
        /// Checks if the payment id exists for the user id in the user service DB
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void VerifyPaymentId(Guid? paymentId)
        {
            Guid userId = GetUserIdFromClaims();

            logger.LogDebug("Received request to verify a payment record at {0} for the user at {1}", paymentId, userId);

            HttpResponseMessage response = client.GetAsync($"http://localhost:5004/api/user/{userId}/payment/{paymentId}/verify").Result;

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("No payment record found at {0} for the user at {1}", paymentId, userId);

                throw new NotFoundException("No payment record has been found for the given id");
            }

            logger.LogDebug("Successfully verify an payment at {0} for the user at {1}", paymentId, userId);
        }
    }
}
