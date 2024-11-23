namespace Entities.Dtos
{
    public class ProductAllResponseDto
    {
        public string Category { get; set; } = string.Empty;

        public List<ProductResponseDto> Products { get; set; }
    }
}
