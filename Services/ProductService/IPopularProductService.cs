using ASP_SPR311.Data.Entities;

namespace ASP_SPR311.Services.ProductService
{
    public interface IPopularProductService
    {
        List<Product> GetTopPopularProducts(int count = 5);
    }
}
