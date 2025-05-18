namespace ASP_SPR311.Services.Kdf
{
    public interface IkdfService
    {
        String DerivedKey(String password, String salt);
    }
}
