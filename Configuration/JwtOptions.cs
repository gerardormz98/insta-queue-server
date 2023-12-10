namespace LiveWaitlistServer.Configuration
{
    public class JwtOptions
    {
        public const string KeyName = "Jwt";

        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}