using ASP_SPR311.Data.Entities;
using ASP_SPR311.Data;

namespace ASP_SPR311.Services.ProductService
{
    public class PopularProductService : IPopularProductService
    {
        private readonly DataContext _context;

        public PopularProductService(DataContext context)
        {
            _context = context;
        }

        public List<Product> GetTopPopularProducts(int count = 5)
        {
            return _context.Products
                .OrderByDescending(p => p.Stock)
                .Take(count)
                .ToList();
        }
    }
}
