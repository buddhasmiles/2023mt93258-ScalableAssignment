using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class WishListItem : BaseModel
    {
        //Below lines establishes a relationship between the tables, "WishList" and "WishListItem"
        
        [ForeignKey("WishListId")]
        public WishList WishList { get; set; }

        public Guid WishListId { get; set; }

        public Guid ProductId { get; set; }
    }
}
