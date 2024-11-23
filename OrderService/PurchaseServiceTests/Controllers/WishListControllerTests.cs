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
    public class WishListControllerTests
    {
        public WishListController controller;

        public void SetUp(RepositoryContext context, Guid userId, bool? isUserIdExists = true, bool? isAllPoductIdsExists = true)
        {
            IWishListRepository wishListRepo = new WishListRepository(context);

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

            ILogger<WishListService> loggerWishListService = new Mock<ILogger<WishListService>>().Object;

            IWishListService wishListService = new WishListService(mapper, wishListRepo, commonService, loggerWishListService);

            ILogger<WishListController> loggerWishListController = new Mock<ILogger<WishListController>>().Object;

            controller = new WishListController(wishListService, loggerWishListController);
        }

        [Fact]
        public void CreateWishList_WhenCreatedByCustomerWithValidValues_Returns201WithIdentityResponseDto()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            WishListCreateDto newWishList = new WishListCreateDto()
            {
                Name = "Work essentials",
                WishListItem = new List<WishListItemCreateDto>
                {
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("0082da15-25e4-454b-a28a-d0b093dbd11d")
                    },
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("1565a22d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            IActionResult result = controller.CreateWishList(newWishList);

            ObjectResult createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().BeOfType<IdentityResponseDto>();

            IdentityResponseDto identityResponse = (IdentityResponseDto)createdResult.Value;
            IActionResult resultGet = controller.GetWishListById(identityResponse.Id);

            ObjectResult okResult = resultGet.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new WishListResponseDto
            {
                Id = identityResponse.Id,
                Name = "Work essentials",
                WishListItem = new List<WishListItemResponseDto>
                {
                    new WishListItemResponseDto
                    {
                        ProductId = Guid.Parse("0082da15-25e4-454b-a28a-d0b093dbd11d")
                    },
                    new WishListItemResponseDto
                    {
                        ProductId = Guid.Parse("1565a22d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            });
        }

        [Fact]
        public void CreateWishList_WhenCreatedWithValidValuesAndUserAccountHasNotBeenFound_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            WishListCreateDto newWishList = new WishListCreateDto()
            {
                Name = "Work essentials",
                WishListItem = new List<WishListItemCreateDto>
                {
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("0082da15-25e4-454b-a28a-d0b093dbd11d")
                    },
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("1565a22d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            Action act = () => controller.CreateWishList(newWishList);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void CreateWishList_WhenCreatedWithValidValuesAndEvenAnyOneOfTheProductIsNotFound_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), true, false);

            WishListCreateDto newWishList = new WishListCreateDto()
            {
                Name = "Work essentials",
                WishListItem = new List<WishListItemCreateDto>
                {
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("0082da15-25e4-454b-a28a-d0b093dbd11d")
                    },
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("1565a22d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            Action act = () => controller.CreateWishList(newWishList);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void CreateWishList_WhenCreatedWithExistingWishListNameForTheUser_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            WishListCreateDto newWishList = new WishListCreateDto()
            {
                Name = "S's Personal",
                WishListItem = new List<WishListItemCreateDto>
                {
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("0082da15-25e4-454b-a28a-d0b093dbd11d")
                    },
                    new WishListItemCreateDto
                    {
                        ProductId = Guid.Parse("1565a22d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            Action act = () => controller.CreateWishList(newWishList);

            act.Should().Throw<ConflictException>().WithMessage("Wish List name already exists");
        }

        [Fact]
        public void GetWishListById_WhenCalledByValidCustomerAndIfTheWishListExists_Returns200WithWishListResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new WishListResponseDto
            {
                Id = Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"),
                Name = "S's Personal",
                WishListItem = new List<WishListItemResponseDto>
                {
                    new WishListItemResponseDto
                    {
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc")
                    },
                    new WishListItemResponseDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                    }
                }
            });
        }

        [Fact]
        public void GetWishListById_WhenCalledByValidCustomerAndIfTheUserDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            Action act = () => controller.GetWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void GetWishListById_WhenCalledByValidCustomerAndIfTheWishListDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetWishListById(Guid.Parse("999637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            act.Should().Throw<NotFoundException>().WithMessage("No wish list exists for the given id");
        }

        [Fact]
        public void GetAllWishListByUserId_WhenCalledByValidCustomerAndIfThereIsAtLeastOneRecord_Returns200WithAListOfWishListResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAllWishListByUserId();

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new List<WishListResponseDto>
            {
                new WishListResponseDto
                {
                    Id = Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"),
                    Name = "S's Personal",
                    WishListItem = new List<WishListItemResponseDto>()
                    {
                        new WishListItemResponseDto
                        {
                            ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc")
                        },
                        new WishListItemResponseDto
                        {
                            ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                        }
                    }
                }
            });
        }

        [Fact]
        public void GetAllWishListByUserId_WhenCalledByValidCustomerAndIfThereIsNoRecord_Returns204()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAllWishListByUserId();

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void GetAllWishListByUserId_WhenCalledByValidCustomerAndIfTheUserAccountDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            Action action = () => controller.GetAllWishListByUserId();

            action.Should().Throw<NotFoundException>();
        }

        //[Fact]
        public void UpdateWishListById_WhenCalledByValidCustomerWithValidValuesAndIfThereIsAWishList_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            WishListUpdateDto updatedWishList = new WishListUpdateDto()
            {
                Name = "Sudharsan's Personal",
                WishListItem = new List<WishListItemUpdateDto>
                {
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                    },
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("9994522d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            IActionResult resultUpdated = controller.UpdateWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"), updatedWishList);

            StatusCodeResult updatedResult = resultUpdated.Should().BeOfType<StatusCodeResult>().Subject;
            updatedResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            IActionResult resultGet = controller.GetWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            ObjectResult okResultGet = resultGet.Should().BeOfType<ObjectResult>().Subject;
            okResultGet.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResultGet.Value.Should().BeEquivalentTo(new WishListResponseDto
            {
                Id = Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"),
                Name = "Sudharsan's Personal",
                WishListItem = new List<WishListItemResponseDto>
                {
                    new WishListItemResponseDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                    },
                    new WishListItemResponseDto
                    {
                        ProductId = Guid.Parse("9994522d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            });
        }

        [Fact]
        public void UpdateWishListById_WhenNoUserAccountHasBeenFoundForTheGivenId_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            WishListUpdateDto updatedWishList = new WishListUpdateDto()
            {
                Name = "Sudharsan's Personal",
                WishListItem = new List<WishListItemUpdateDto>
                {
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                    },
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("9994522d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            Action act = () => controller.UpdateWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"), updatedWishList);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void UpdateWishListById_WhenAtLeastOneOfTheProductHasNotBeenFound_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), true, false);

            WishListUpdateDto updatedWishList = new WishListUpdateDto()
            {
                Name = "Sudharsan's Personal",
                WishListItem = new List<WishListItemUpdateDto>
                {
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                    },
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("9994522d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            Action act = () => controller.UpdateWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"), updatedWishList);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void UpdateWishListById_WhenUpdatedWithExistingWishListName_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            WishListUpdateDto updatedWishList = new WishListUpdateDto()
            {
                Name = "S's Personal",
                WishListItem = new List<WishListItemUpdateDto>
                {
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                    },
                    new WishListItemUpdateDto
                    {
                        ProductId = Guid.Parse("9994522d-dc63-45d9-8e2f-704313937de9")
                    }
                }
            };

            Action act = () => controller.UpdateWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"), updatedWishList);

            act.Should().Throw<ConflictException>().WithMessage("Wish list name already exists");
        }

        [Fact]
        public void DeleteWishListById_WhenCalledByValidCustomerToDeleteAnExistingWishList_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult resultDelete = controller.DeleteWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            Action act = () => controller.GetWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            StatusCodeResult okResult = resultDelete.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            act.Should().Throw<NotFoundException>().WithMessage("No wish list exists for the given id");
        }

        [Fact]
        public void DeleteWishListById_WhenCalledByValidCustomerToDeleteAWishListThatDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.DeleteWishListById(Guid.Parse("123637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            act.Should().Throw<NotFoundException>().WithMessage("No wish list has been found for the given id");
        }

        [Fact]
        public void DeleteWishListById_WhenTheUserAccountDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), false);

            Action act = () => controller.DeleteWishListById(Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }
    }
}
