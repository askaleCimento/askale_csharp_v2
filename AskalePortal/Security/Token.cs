namespace AskalePortal.API.Security
{
    public class Token
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }

        public DateTime Expiration { get; set; }
    }
}
