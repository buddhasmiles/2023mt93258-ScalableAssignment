using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class AddToCartDto
    {
        [Required(ErrorMessage = "This field is required")]
        public List<AddToCartProductDto> Products { get; set; } 
    }
}
