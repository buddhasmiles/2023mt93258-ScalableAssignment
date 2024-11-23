using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        public int Price { get; set; }


        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; } = string.Empty;


        //[Required(ErrorMessage = "This field is required")]
        public string Image { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        public int AvailableCount { get; set; }


        [Required(ErrorMessage = "This field is required")]
        public bool Visibility { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Clothing and Apparel|Footwear and Shoes|Electronics and Gadgets|Games and Toys|
        Veterinary and Pet Items|Stationery|Hand and Power Tools|Furniture|Sports)$", 
        ErrorMessage = "The field can only be any one of the following, Clothing and Apparel, Footwear and Shoes, " +
        "Electronics and Gadgets, Games and Toys, Veterinary and Pet Items, Stationery, Hand and Power Tools, " +
        "Furniture, Sports")]
        public string Category { get; set; } = string.Empty;
    }
}
