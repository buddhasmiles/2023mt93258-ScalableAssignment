using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class WishListItemCreateDto
    {
        [Required(ErrorMessage = "This field is required")]
        public Guid ProductId { get; set; }
    }
}
