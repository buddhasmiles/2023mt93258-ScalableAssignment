using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class CartProductUpdateDto
    {
        [Required(ErrorMessage = "This field is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 10, ErrorMessage = "Quantity should not exceed 10")]
        public int Quantity { get; set; }
    }
}
