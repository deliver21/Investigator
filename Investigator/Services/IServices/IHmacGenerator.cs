namespace Investigator.Services.IServices
{
    public interface IHmacGenerator
    {
        public string GenerateHmac(int message);
    }
}
