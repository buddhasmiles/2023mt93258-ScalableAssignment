using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) { }

        public override int SaveChanges()
        {
            var selectedEntityList = ChangeTracker.Entries()
                                    .Where(x => x.Entity is BaseModel &&
                                    (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in selectedEntityList)
            {
                ((BaseModel)entity.Entity).DateCreated = DateTime.Now;
                ((BaseModel)entity.Entity).DateUpdated = DateTime.Now;
            }

            return base.SaveChanges();
        }
        
        // Creates table for Cart, Order, OrderItem, WishList, WishListItem

        public DbSet<Cart> Cart { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<WishList> WishList { get; set; }

        public DbSet<WishListItem> WishListItem { get; set; }

    }
}
