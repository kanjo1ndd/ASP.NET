using Microsoft.AspNetCore.Mvc;

namespace ASP_SPR311.Models.Home
{
    public class ReviewAjaxFormModel
    {
        [FromForm(Name = "author")]
        public String Author { get; set; } = null!;

        [FromForm(Name = "comment")]
        public String Comment { get; set; } = null!;

        [FromForm(Name = "rating")]
        public int Rating { get; set; }

        [FromForm(Name = "date")]
        public String Date { get; set; } = null!;
    }
}
