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
    public class CartControllerTests
    {
        public CartController controller;

        public void SetUp(RepositoryContext context, Guid userId, bool? isUserIdExists = true, bool? isAllPoductIdsExists = true)
        {
            ICartRepository cartRepo = new CartRepository(context);

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

            if (!(bool)isUserIdExists)
            {
                verifyUser = HttpStatusCode.NotFound;
            }

            if (!(bool)isAllPoductIdsExists)
            {
                verifyProduct = HttpStatusCode.NotFound;
            }

            mockClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                      .Returns(Task.FromResult(new HttpResponseMessage
                      {
                          StatusCode = verifyUser
                      }));

            mockClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                      .Returns(Task.FromResult(new HttpResponseMessage
                      {
                          StatusCode = verifyProduct
                      }));

            ICommonService commonService = new CommonService(httpContextAccessor, httpClientFactory, mockClient.Object, loggerCommonService);

            ILogger<CartService> loggerCartService = new Mock<ILogger<CartService>>().Object;

            ICartService cartService = new CartService(mapper, commonService, cartRepo, loggerCartService);

            ILogger<CartController> loggerCartController = new Mock<ILogger<CartController>>().Object;

            controller = new CartController(cartService, loggerCartController);
        }

        [Fact]
        public void AddProductToCart_WhenCalledByValidCustomerWithValidValues_Returns201WithIdentityResponseDto()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddToCartDto addToCart = new AddToCartDto
            {
                Products = new List<AddToCartProductDto>()
                {
                    new AddToCartProductDto
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 2
                    }
                }
            };

            IActionResult result = controller.AddProductToCart(addToCart);

            StatusCodeResult createdResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);

            IActionResult resultGet = controller.GetCartItemsByUserId();

            ObjectResult okResult = resultGet.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new List<CartResponseDto>
            {
                new CartResponseDto
                {
                    ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                    Quantity = 2
                },
            });
        }

        [Fact]
        public void AddProductToCart_WhenTheUserAccountDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            AddToCartDto addToCart = new AddToCartDto
            {
                Products = new List<AddToCartProductDto>()
                {
                    new AddToCartProductDto
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 2
                    }
                }
            };

            Action act = () => controller.AddProductToCart(addToCart);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void AddProductToCart_WhenAnyOneOfTheProductDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), true, false);

            AddToCartDto addToCart = new AddToCartDto
            {
                Products = new List<AddToCartProductDto>()
                {
                    new AddToCartProductDto
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 2
                    }
                }
            };

            Action act = () => controller.AddProductToCart(addToCart);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void GetCartItemsByUserId_WhenCalledByValidCustomerAndIfThereIsAtLeastOneRecord_Returns200WithAListOfCartResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetCartItemsByUserId();

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new List<CartResponseDto>
            {
                new CartResponseDto
                {
                    ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                    Quantity = 1
                },
                new CartResponseDto
                {
                    ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                    Quantity = 1
                }
            });
        }

        [Fact]
        public void GetCartItemsByUserId_WhenCalledByValidCustomerAndIfThereAreNoRecords_Returns204()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetCartItemsByUserId();

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void GetCartItemsByUserId_WhenUserAccountDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            Action act = () => controller.GetCartItemsByUserId();

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void UpdateCartById_WhenCalledByValidCustomerWithValidValuesAndIfThereAreIsAtLeastOneProduct_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddToCartUpdateDto updatedCart = new AddToCartUpdateDto()
            {
                Products = new List<CartProductUpdateDto>
                {
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 1   
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 3
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("12345bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 1
                    }
                }
            };

            IActionResult resultUpdated = controller.UpdateCartByUserId(updatedCart);

            StatusCodeResult updatedResult = resultUpdated.Should().BeOfType<StatusCodeResult>().Subject;
            updatedResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            IActionResult resultGet = controller.GetCartItemsByUserId();

            ObjectResult okResultGet = resultGet.Should().BeOfType<ObjectResult>().Subject;
            okResultGet.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResultGet.Value.Should().BeEquivalentTo(new List<CartResponseDto>
            {
                new CartResponseDto
                {
                    ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                    Quantity = 1
                },
                new CartResponseDto
                {
                    ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                    Quantity = 3
                },
                new CartResponseDto
                {
                    ProductId = Guid.Parse("12345bd6-278e-4171-b0f1-6f2233b340b2"),
                    Quantity = 1
                }
            });
        }

        [Fact]
        public void UpdateCartById_WhenCalledWithAUserAccountThatDoesNotExists_ThrowsNotFoundExceptino()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            AddToCartUpdateDto updatedCart = new AddToCartUpdateDto()
            {
                Products = new List<CartProductUpdateDto>
                {
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 1
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 3
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("12345bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 1
                    }
                }
            };

            Action act = () => controller.UpdateCartByUserId(updatedCart);
            
            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void UpdateCartById_WhenNoProductHasBeenAddedToTheCart_ThrowsBadRequestException()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddToCartUpdateDto updatedCart = new AddToCartUpdateDto()
            {
                Products = new List<CartProductUpdateDto>
                {
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 1
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 3
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("12345bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 1
                    }
                }
            };

            Action act = () => controller.UpdateCartByUserId(updatedCart);

            act.Should().Throw<BadRequestException>().WithMessage("No product has been added to the cart");
        }

        [Fact]
        public void UpdateCartById_WhenOneOrMoreProductDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), true, false);

            AddToCartUpdateDto updatedCart = new AddToCartUpdateDto()
            {
                Products = new List<CartProductUpdateDto>
                {
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 1
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 3
                    },
                    new CartProductUpdateDto()
                    {
                        ProductId = Guid.Parse("12345bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 1
                    }
                }
            };

            Action act = () => controller.UpdateCartByUserId(updatedCart);

            act.Should().Throw<NotFoundException>();
        }

    }
}
