using ASP_SPR311.Data;
using ASP_SPR311.Data.Entities;
using ASP_SPR311.Models.Shop;
using ASP_SPR311.Models.User;
using ASP_SPR311.Services.ProductService;
using ASP_SPR311.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;

namespace ASP_SPR311.Controllers
{
    public class ShopController(DataAccessor dataAccessor, DataContext dataContext, IStorageService storageService, IPopularProductService popularProductService) : Controller
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly DataContext _dataContext = dataContext;
		private readonly IStorageService _storageService = storageService;
        private readonly IPopularProductService _popularProductService = popularProductService;
        public IActionResult Index()
        {
            ShopIndexViewModel viewModel = new()
            {
                Categories = _dataAccessor.AllCategories()
            };

            return View(viewModel);
        }
        public IActionResult Category([FromRoute] String id)
        {
            ShopCategoryViewModel viewModel = new()
            {
                Category = _dataContext
                .Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Slug == id)
            };

            return View(viewModel);
        }

        public ViewResult Product([FromRoute] String id)
        {
            String controllerName = ControllerContext.ActionDescriptor.ControllerName;
            ShopProductViewModel viewModel = new()
            {
                Product = _dataContext
                .Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id),
                BreadCrumbs = new() {
                    new BreadCrumb() { Title = "Домашня", Url = "/" },
                    new BreadCrumb() { Title = "Крамниця", Url = $"/{controllerName}" },
                }
            };
            if(viewModel.Product != null)
            {
                viewModel.BreadCrumbs.Add(
                    new BreadCrumb() {
                        Title = viewModel.Product.Category.Name,
                        Url = $"/{controllerName}/{nameof(Category)}/{viewModel.Product.Category.Slug}",
                    });
				viewModel.BreadCrumbs.Add(
					new BreadCrumb()
					{
						Title = viewModel.Product.Name,
						Url = $"/{controllerName}/{nameof(Product)}/{viewModel.Product.Slug ?? viewModel.Product.Id.ToString()}",
					});
			}
            return View(viewModel);
        }

        public ViewResult Cart()
        {
            String? uaId = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

            Cart? cart = _dataContext
                .Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserAccessId.ToString() == uaId);

            List<Product> recommendedProducts = new List<Product>();

            if (cart?.CartItems != null && cart.CartItems.Any())
            {
                var categoryIds = cart.CartItems
                    .Select(ci => ci.Product.CategoryId)
                    .Distinct()
                    .ToList();

                var cartProductIds = cart.CartItems.Select(ci => ci.ProductId).ToList();

                recommendedProducts = _dataContext.Products
                    .Where(p => categoryIds.Contains(p.CategoryId) && !cartProductIds.Contains(p.Id))
                    .Take(4)
                    .ToList();
            }

            ShopCartViewModel viewModel = new()
            {
                Cart = cart == null ? null :
                    cart with { 
                        CartItems = [.. cart.CartItems
                        .Select(ci => ci with {
                            Product = ci.Product with {
                                ImagesCsv = String.IsNullOrEmpty(ci.Product.ImagesCsv)
                                ? "/Shop/Image/np-image.jpg"
                                : ci.Product.ImagesCsv = String.Join(',',
                                    ci.Product.ImagesCsv
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(img => "/Shop/Image/" + img))
                            }
                        })]
                    },
                RecommendedProducts = recommendedProducts
            };

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AddToCart([FromForm] string productId, [FromForm] string uaId)
        {
            if (!Guid.TryParse(productId, out Guid productGuid) || !Guid.TryParse(uaId, out Guid userAccessGuid))
            {
                return Json(new { Status = 400, Message = "Невірний формат ідентифікатора" });
            }

            Product? product = _dataContext.Products.FirstOrDefault(p => p.Id == productGuid);
            if (product == null)
            {
                return Json(new { Status = 404, Message = "Продукт не знайдено" });
            }

            Cart? cart = _dataContext.Carts
                .FirstOrDefault(c => c.UserAccessId == userAccessGuid);

            if (cart == null)
            {
                cart = new Cart()
                {
                    Id = Guid.NewGuid(),
                    UserAccessId = userAccessGuid,
                    OpenAt = DateTime.Now,
                    Price = 0
                };
                _dataContext.Carts.Add(cart);
            }

            CartItem? cartItem = _dataContext.CartItems
                .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == productGuid);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                cartItem.Price += product.Price;
            }
            else
            {
                cartItem = new CartItem()
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = 1,
                    Price = product.Price,
                };
                _dataContext.CartItems.Add(cartItem);
            }

            cart.Price = _dataContext.CartItems.Where(ci => ci.CartId == cart.Id).Sum(ci => ci.Price);

            _dataContext.SaveChanges();

            string positionText = GetUkrainianPlural(cartItem.Quantity, "позиція", "позиції", "позицій");

            return Json(new { Status = 200, Message = $"Додано {cartItem.Quantity} {positionText} у кошик", CartTotal = cart.Price,
                CartItems = _dataContext.CartItems
        .Where(ci => ci.CartId == cart.Id)
        .Select(ci => new { ci.ProductId, ci.Quantity, ci.Price })
            });
        }

        public JsonResult ModifyCartItem([FromQuery] String cartId, [FromQuery] int delta)
        {
            if(delta == 0)
            {
                return Json(new { Status = 400, Message = "No action needed for 0 delta" });
            }
            Guid cartGuid;
            try { cartGuid = Guid.Parse(cartId); }
            catch { return Json(new { Status = 400, Message = "Invalid cartId format: UUID expected" }); }

            CartItem? cartItem = _dataContext.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefault(ci => ci.Id == cartGuid);
            if(cartItem == null)
            {
                return Json(new { Status = 404, Message = "No item with requested cartId" });
            }
            int newQuantity = cartItem.Quantity + delta;
            if (newQuantity < 0)
            {
				return Json(new { Status = 400, Message = "Invalid delta: negative total quantity" });
			}
			if (newQuantity > cartItem.Product.Stock)
			{
				return Json(new { Status = 422, Message = "Delta too large: stock limit exceeded" });
			}
            if(newQuantity == 0)
            {
                cartItem.Cart.Price -= cartItem.Price;
                _dataContext.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Cart.Price += delta * cartItem.Product.Price;
				cartItem.Price += delta * cartItem.Product.Price;
                cartItem.Quantity = newQuantity;
			}
            _dataContext.SaveChanges();
			return Json(new { Status = 200, Message = "Modifed" });
        }
        private string GetUkrainianPlural(int number, string singular, string dual, string plural)
        {
            if (number % 10 == 1 && number % 100 != 11)
                return singular;
            else if (number % 10 >= 2 && number % 10 <= 4 && (number % 100 < 10 || number % 100 >= 20))
                return dual;
            else
                return plural;
        }

        [HttpPost]
        public IActionResult PurchaseCart(Guid cartId)
        {
            var cart = _dataContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.Id == cartId);

            if (cart == null)
            {
                Console.WriteLine("Корзина не найдена!");
                return RedirectToAction("Cart");
            }

            cart.CloseAt = DateTime.UtcNow;
            cart.IsCanceled = 0;

            foreach (var item in cart.CartItems)
            {
                item.Product.Stock -= item.Quantity;
            }

            _dataContext.SaveChanges();

            TempData["SuccessMessage"] = "Замовлення успішно оформлене!";
            return RedirectToAction("Cart");
        }

        public IActionResult PopularProducts()
        {
            var products = _popularProductService.GetTopPopularProducts();
            return PartialView("_PopularProducts", products);
        }

        [HttpPost]
        public IActionResult CancelCart(Guid cartId)
        {
            var cart = _dataContext
                .Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.Id == cartId);

            if (cart == null)
            {
                Console.WriteLine("Корзина не найдена!");
                return RedirectToAction("Cart");
            }

            if (cart.IsCanceled == 1)
            {
                TempData["ErrorMessage"] = "Це замовлення вже було скасоване!";
                return RedirectToAction("Cart");
            }

            foreach (var item in cart.CartItems)
            {
                item.Product.Stock += item.Quantity;
            }

            cart.CloseAt = DateTime.UtcNow;
            cart.IsCanceled = 1;

            _dataContext.SaveChanges();

            TempData["ErrorMessage"] = "Замовлення скасовано!";
            return RedirectToAction("Cart");
        }
        public IActionResult Image([FromRoute] String id)
        {
            var path = _storageService.GetRealPath(id);

            if (!System.IO.File.Exists(path))
            {
                return Content("Файл не знайдено. Путь: " + path);
            }

            return File(System.IO.File.ReadAllBytes(path), "image/jpeg", false);
        }
    }
}
