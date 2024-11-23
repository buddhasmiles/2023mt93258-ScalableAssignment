namespace Entities.Dtos
{
    public class OrderItemResponseDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public string Status { get; set; } = String.Empty;
    }
}
