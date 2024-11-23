namespace Entities.Dtos
{
    public class TokenDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

    }
}
