using OrderService.Controllers;
using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Repository;
using Services;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace PurchaseServiceTests.Controllers
{
    public class OrderControllerTests
    {
        public OrderController controller;

        public void SetUp(RepositoryContext context, Guid userId, Guid? addressId = null, Guid? paymentId = null, 
                          bool? isUserIdExists = true, bool? isAllPoductIdsExists = true, bool? isAddressIdExists = true,
                          bool? isPaymentExists = true)
        {
            ICartRepository cartRepo = new CartRepository(context);

            IOrderRepository orderRepo = new OrderRepository(context);

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();

            Claim userIdClaim = new Claim("user_id", userId.ToString());
            ClaimsIdentity identity = new ClaimsIdentity(new[] { userIdClaim }, "BasicAuthentication");
            ClaimsPrincipal contextUser = new ClaimsPrincipal(identity);
            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            HttpContextAccessor httpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = httpContext
            };

            IHttpClientFactory httpClientFactory = new Mock<IHttpClientFactory>().Object;

            ILogger<CommonService> loggerCommonService = new Mock<ILogger<CommonService>>().Object;

            var mockClient = new Mock<IHttpClientService>();

            HttpStatusCode verifyUser = HttpStatusCode.OK;
            HttpStatusCode verifyProduct = HttpStatusCode.OK;
            HttpStatusCode verifyAddress = HttpStatusCode.OK;
            HttpStatusCode verifyPayment = HttpStatusCode.OK;

            if (!(bool)isUserIdExists)
            {
                verifyUser = HttpStatusCode.NotFound;
            }

            if (!(bool)isAllPoductIdsExists)
            {
                verifyProduct = HttpStatusCode.NotFound;
            }

            if (!(bool)isAddressIdExists)
            {
                verifyAddress = HttpStatusCode.NotFound;
            }

            if (!(bool)isPaymentExists)
            {
                verifyPayment = HttpStatusCode.NotFound;
            }

            mockClient.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains($"https://localhost:7004/api/user/{userId}/verify"))))
                      .Returns(Task.FromResult(new HttpResponseMessage
                      {
                          StatusCode = verifyUser
                      }));

            mockClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                      .Returns(Task.FromResult(new HttpResponseMessage
                      {
                          StatusCode = verifyProduct
                      }));

            mockClient.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains($"https://localhost:7004/api/user/{userId}/address/{addressId}/verify"))))
                      .Returns(Task.FromResult(new HttpResponseMessage
                      {
                          StatusCode = verifyAddress
                      }));

            mockClient.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains($"https://localhost:7004/api/user/{userId}/payment/{paymentId}/verify"))))
                      .Returns(Task.FromResult(new HttpResponseMessage
                      {
                          StatusCode = verifyPayment
                      }));

            ICommonService commonService = new CommonService(httpContextAccessor, httpClientFactory, mockClient.Object, loggerCommonService);

            ILogger<Services.OrderService> loggerOrderService = new Mock<ILogger<Services.OrderService>>().Object;

            IOrderService orderService = new Services.OrderService(mapper, commonService, cartRepo, orderRepo, loggerOrderService);

            ILogger<OrderController> loggerOrderController = new Mock<ILogger<OrderController>>().Object;

            controller = new OrderController(orderService, loggerOrderController);
        }

        [Fact]
        public void CreateOrder_WhenCalledByValidCustomerWithAtLeastOneProductAddedToTheCart_Returns200WithIdentityResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"));

            CheckOutFromCartDto checkOut = new CheckOutFromCartDto 
            {
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "COD"
            };

            IActionResult result = controller.CreateOrder(checkOut);

            ObjectResult createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().BeOfType<IdentityResponseDto>();

            IdentityResponseDto identityResponse = (IdentityResponseDto)createdResult.Value;
            IActionResult resultGet = controller.GetOrderById(identityResponse.Id);

            ObjectResult okResult = resultGet.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new OrderResponseDto
            {
                Id = identityResponse.Id,
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "COD",
                OrderItem = new List<OrderItemResponseDto>
                {
                    new OrderItemResponseDto
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 1,
                        Status = "Processing"
                    },
                    new OrderItemResponseDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 1,
                        Status = "Processing"
                    }
                }
            });
        }

        [Fact]
        public void CreateOrder_WhenUserAccountDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), isUserIdExists: false);

            CheckOutFromCartDto checkOut = new CheckOutFromCartDto
            {
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "COD"
            };

            Action act = () => controller.CreateOrder(checkOut);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void CreateOrder_WhenNoProductAddedToTheCart_ThrowsBadRequestException()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            CheckOutFromCartDto checkOut = new CheckOutFromCartDto
            {
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "COD"
            };

            Action act = () => controller.CreateOrder(checkOut);

            act.Should().Throw<BadRequestException>().WithMessage("No product exists in cart");
        }

        [Fact]
        public void CreateOrder_WhenAddressIdDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                  Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"), isAddressIdExists: false);

            CheckOutFromCartDto checkOut = new CheckOutFromCartDto
            {
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "COD"
            };

            Action act = () => controller.CreateOrder(checkOut);

            act.Should().Throw<NotFoundException>().WithMessage("No address has been found for the given id");
        }

        [Fact]
        public void CreateOrder_WhenPaymentIdDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), userId: Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                  addressId: Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"), paymentId: Guid.Parse("777ee1ec-35c7-41eb-affb-9f60b8eb713d"), 
                  isPaymentExists: false);

            CheckOutFromCartDto checkOut = new CheckOutFromCartDto
            {
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "UPI",
                PaymentId = Guid.Parse("777ee1ec-35c7-41eb-affb-9f60b8eb713d")
            };

            Action act = () => controller.CreateOrder(checkOut);

            act.Should().Throw<NotFoundException>().WithMessage("No payment record has been found for the given id");
        }

        [Fact]
        public void GetOrderById_WhenCalledWithValidOrderIdForTheUser_Returns200WithOrderResponseDto()
        {
            SetUp(context: TestHelper.GetContextWithRecords(), userId: Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetOrderById(Guid.Parse("b2e8d258-bddc-4699-b902-b5988e43d4e2"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new OrderResponseDto
            {
                Id = Guid.Parse("b2e8d258-bddc-4699-b902-b5988e43d4e2"),
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "UPI",
                PaymentId = Guid.Parse("1fe1485b-8bd2-41fb-a55a-630c16c2d6d7"),
                OrderItem = new List<OrderItemResponseDto>()
                {
                    new OrderItemResponseDto
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 1,
                        Status = "Processing"
                    },
                    new OrderItemResponseDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 1,
                        Status = "Processing"
                    }
                }
            });
        }

        [Fact]
        public void GetOrderById_WhenUserAccountDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(context: TestHelper.GetContextWithRecords(), userId: Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), isUserIdExists: false);

            Action act = () => controller.GetOrderById(Guid.Parse("b2e8d258-bddc-4699-b902-b5988e43d4e2"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void GetOrderById_WhenNoOrderExistsForTheUser_ThrowsNotFoundException()
        {
            SetUp(context: TestHelper.GetContextWithRecords(), userId: Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetOrderById(Guid.Parse("12e8d258-bddc-4699-b902-b5988e43d4e2"));

            act.Should().Throw<NotFoundException>().WithMessage("No order exists for the given id");
        }

        [Fact]
        public void GetAllOrderDetails_WhenCalledWithValidUserAndIfThereIsAtLeastOneRecord_Returns200WithListOfOrderResponseDto()
        {
            SetUp(context: TestHelper.GetContextWithRecords(), userId: Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAllOrderDetails();

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new List<OrderResponseDto>
            {
                new OrderResponseDto
                {
                    Id = Guid.Parse("b2e8d258-bddc-4699-b902-b5988e43d4e2"),
                    AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                    PaymentMethod = "UPI",
                    PaymentId = Guid.Parse("1fe1485b-8bd2-41fb-a55a-630c16c2d6d7"),
                    OrderItem = new List<OrderItemResponseDto>()
                    {
                        new OrderItemResponseDto
                        {
                            ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                            Quantity = 1,
                            Status = "Processing"
                        },
                        new OrderItemResponseDto
                        {
                            ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                            Quantity = 1,
                            Status = "Processing"
                        }
                    }
                }
            });
        }

        [Fact]
        public void GetAllOrderDetails_WhenCalledWithValidUserAndIfThereAreNoRecords_Returns204()
        {
            SetUp(context: TestHelper.GetEmptyContext(), userId: Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAllOrderDetails();

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void GetAllOrderDetails_WhenCalledWithUserAccountThatDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(context: TestHelper.GetEmptyContext(), userId: Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), isUserIdExists: false);

            Action act = () => controller.GetAllOrderDetails();

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }
    }
}
