namespace Entities.Dtos
{
    public class WishListResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<WishListItemResponseDto> WishListItem { get; set; } = new List<WishListItemResponseDto>();
    }
}
