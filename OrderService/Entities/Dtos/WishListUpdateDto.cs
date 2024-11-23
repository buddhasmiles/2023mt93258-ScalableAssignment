namespace Entities.Dtos
{
    public class WishListUpdateDto
    {
        public string? Name { get; set; } = string.Empty;

        public List<WishListItemUpdateDto>? WishListItem { get; set; } = new List<WishListItemUpdateDto>();
    }
}
