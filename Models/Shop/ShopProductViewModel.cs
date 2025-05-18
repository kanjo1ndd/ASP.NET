using ASP_SPR311.Data.Entities;
using ASP_SPR311.Models.User;

namespace ASP_SPR311.Models.Shop
{
	public class ShopProductViewModel
	{
		public Product? Product { get; set; }
		public List<BreadCrumb> BreadCrumbs { get; set; } = [];
	}
}
