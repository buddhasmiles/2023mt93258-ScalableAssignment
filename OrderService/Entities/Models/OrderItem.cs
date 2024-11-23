using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class OrderItem : BaseModel
    {
        //Below lines establishes a relationship between the tables, "Order" and "OrderItem"

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public string Status { get; set; } = "Processing";
    }
}
