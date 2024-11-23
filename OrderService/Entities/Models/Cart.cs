namespace Entities.Models
{
    public class Cart : BaseModel
    {
        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
