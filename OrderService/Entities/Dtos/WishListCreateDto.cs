using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class WishListCreateDto
    {
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        public List<WishListItemCreateDto> WishListItem { get; set; } 
    }
}
