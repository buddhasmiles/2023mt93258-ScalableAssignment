namespace Entities.Models
{

    /// <summary>
    /// Contains all the product information
    /// </summary>
    public class Product : BaseModel
    {
        public string Name { get; set; } = string.Empty;

        public int Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public byte[] Image { get; set; }

        public int AvailableCount { get; set; }

        public bool Visibility { get; set; }

        public string Category { get; set; } = string.Empty;
    }
}
