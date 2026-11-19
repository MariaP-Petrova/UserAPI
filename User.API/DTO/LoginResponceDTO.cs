namespace User.API.DTO
{
    public class LoginResponceDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
