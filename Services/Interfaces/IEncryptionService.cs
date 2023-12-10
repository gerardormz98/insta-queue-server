namespace LiveWaitlistServer.Services.Interfaces
{
    public interface IEncryptionService
    {
        public string Encrypt(string value);
        public string Decrypt(string value);
    }
}