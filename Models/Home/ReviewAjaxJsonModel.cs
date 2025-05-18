using Microsoft.AspNetCore.Mvc;

namespace ASP_SPR311.Models.Home
{
    public class ReviewAjaxJsonModel
    {
        [ModelBinder(Name = "author")]
        public string Author { get; set; } = null!;

        [ModelBinder(Name = "comment")]
        public string Comment { get; set; } = null!;

        [ModelBinder(Name = "rating")]
        public int Rating { get; set; }

        [ModelBinder(Name = "date")]
        public string Date { get; set; } = null!;
    }
}
