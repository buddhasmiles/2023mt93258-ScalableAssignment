using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class WishListItemUpdateDto
    {
        [Required(ErrorMessage = "This field is required")]
        public Guid ProductId { get; set; }
    }
}
