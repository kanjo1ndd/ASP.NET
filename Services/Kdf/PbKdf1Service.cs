namespace ASP_SPR311.Services.Kdf
{
    public class PbKdf1Service : IkdfService
    {
        private readonly int iterationCount = 3;
        private readonly int dkLength = 20;
        public string DerivedKey(string password, string salt)
        {
            String t = password + salt;
            for(int i = 0; i < iterationCount; i++)
            {
                t = Hash(t);
            }
            int[] a = [1, 2, 3];
            int[] b = [..a, 4, 5];
            return t[..dkLength];
        }

        private static String Hash(String input) =>
            Convert.ToHexString(
            System.Security.Cryptography.SHA1.HashData(
                System.Text.Encoding.UTF8.GetBytes(input)
                )
            );
    }
}
