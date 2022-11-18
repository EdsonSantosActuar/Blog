namespace Blog;

public static class Configuration
{
    public static string JwtKey = "MTBjNDgzODUtNzk5Yy00ZWE4LWE2MzUtNzZjOTgxMDZkM2Jl";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "MmIwNGVlZTAtMjJhYS00YjlkLWFkZjktZDc0MTc1ZDlkYjky";
    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}