using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace PurchaseServiceTests
{
    public class TestHelper
    {
        /// <summary>
        /// Seeds the in memory database with records
        /// </summary>
        /// <returns></returns>
        public static RepositoryContext GetContextWithRecords()
        {
            var option = new DbContextOptionsBuilder<RepositoryContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new RepositoryContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            // Wish List, Cart, Order | User - Sudharsan

            context.Add(new WishList()
            {
                Id = Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"),
                UserId = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                Name = "S's Personal",
                WishListItem = new List<WishListItem>()
                {
                    new WishListItem
                    {
                        Id = Guid.Parse("bed141e2-4996-46f0-b8a1-4b7c9969ee5e"),
                        WishListId = Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"),
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc")
                    },
                    new WishListItem
                    {                        
                        Id= Guid.Parse("d0d714a9-ed38-401c-80f3-97f5958558e5"),
                        WishListId = Guid.Parse("392637ee-0109-4a0a-9b0e-d884eb5a4dc8"),
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2")
                    }
                }
            });

            context.Add(new Cart()
            {
                UserId = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                Quantity = 1
            });

            context.Add(new Cart()
            {
                UserId = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                Quantity = 1
            });

            context.Add(new Order()
            {
                Id = Guid.Parse("b2e8d258-bddc-4699-b902-b5988e43d4e2"),
                UserId = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                AddressId = Guid.Parse("897ee1ec-35c7-41eb-affb-9f60b8eb713d"),
                PaymentMethod = "UPI",
                PaymentId = Guid.Parse("1fe1485b-8bd2-41fb-a55a-630c16c2d6d7"),
                OrderItem = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        OrderId = Guid.Parse("b2e8d258-bddc-4699-b902-b5988e43d4e2"),
                        ProductId = Guid.Parse("112ff65c-e52d-47e7-b409-18ef76672edc"),
                        Quantity = 1,
                        Status = "Processing"
                    },
                    
                    new OrderItem
                    {
                        OrderId = Guid.Parse("b2e8d258-bddc-4699-b902-b5988e43d4e2"),
                        ProductId = Guid.Parse("e3500bd6-278e-4171-b0f1-6f2233b340b2"),
                        Quantity = 1,
                        Status = "Processing"
                    }
                }
            });

            context.SaveChanges();

            return context;
        }

        /// <summary>
        /// Seeds the in memory database with no records
        /// </summary>
        /// <returns></returns>
        public static RepositoryContext GetEmptyContext()
        {
            var option = new DbContextOptionsBuilder<RepositoryContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new RepositoryContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }


    }
}
