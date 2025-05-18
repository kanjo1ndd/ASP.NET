namespace ASP_SPR311.Services.Timestamp
{
	public class OtpService
	{
		private readonly Random _random = new();

		public string GenerateOtp(int length)
		{
			if (length <= 0)
				throw new ArgumentException("Error");

			return new string(Enumerable.Range(0, length)
				.Select(_ => (char)('0' + _random.Next(10)))
				.ToArray());
		}
	}
}
