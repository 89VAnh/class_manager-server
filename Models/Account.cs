namespace Models
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }

        public string? token { get; set; }
    }

    public class AppSettings
    {
        public string Secret { get; set; }
    }
}