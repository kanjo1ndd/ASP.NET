namespace ASP_SPR311.Models.User
{
    public class UserSignupFormModel
    {
        public String UserName { get; set; } = null!;
        public String UserEmail { get; set; } = null!;
        public String UserPhone { get; set; } = null!;
        public String UserLogin { get; set; } = null!;
        public String UserPassword { get; set; } = null!;
        public String UserRepeat { get; set; } = null!;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public DateTime? BirthDate { get; set; }
        public int? ClothingSize { get; set; }
        public double? ShoeSize { get; set; }
        public double? RingSize { get; set; }
        public String? SocialNetworkUrl { get; set; }
    }
}
