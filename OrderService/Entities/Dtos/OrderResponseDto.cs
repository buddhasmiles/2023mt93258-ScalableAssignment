using Entities.Models;

namespace Entities.Dtos
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }

        public Guid AddressId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public Guid? PaymentId { get; set; }

        public ICollection<OrderItemResponseDto>? OrderItem { get; set; } = new List<OrderItemResponseDto>();
    }
}
