namespace Entities.Dtos
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public int AvailableCount { get; set; }

        public string Category { get; set; } = string.Empty;
    }
}
