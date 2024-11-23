namespace Entities.Models
{
    public class Order : BaseModel
    {
        public Guid UserId { get; set; }

        public Guid AddressId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public Guid? PaymentId { get; set; }

        public int Total { get; set; } = 0;

        public ICollection<OrderItem>? OrderItem { get; set; } = new List<OrderItem>();
    }
}
