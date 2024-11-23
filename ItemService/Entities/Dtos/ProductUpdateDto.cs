using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class ProductUpdateDto
    {
        public string? Name { get; set; } 

        public int? Price { get; set; }

        public string? Description { get; set; } 

        public string? Image { get; set; } 

        public int? AvailableCount { get; set; }

        public bool? Visibility { get; set; }

        [RegularExpression(@"^(Clothing and Apparel|Footwear and Shoes|Electronics and Gadgets|Games and Toys|
        Veterinary and Pet Items|Stationery|Hand and Power Tools|Furniture|Sports)$",
        ErrorMessage = "The field can only be any one of the following, Clothing and Apparel, Footwear and Shoes, " +
        "Electronics and Gadgets, Games and Toys, Veterinary and Pet Items, Stationery, Hand and Power Tools, " +
        "Furniture, Sports")]
        public string? Category { get; set; } 
    }
}
