namespace Entities.Models
{
    public class WishList : BaseModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Quantity { get; set; } = 0;

        public List<WishListItem>? WishListItem { get; set; } = new List<WishListItem>();
    }
}
