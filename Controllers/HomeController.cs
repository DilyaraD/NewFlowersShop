using Microsoft.AspNetCore.Mvc;
using NewFlowersShop.Models;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace NewFlowersShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NewFlowersShopContext _context;
        public HomeController(ILogger<HomeController> logger, NewFlowersShopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var newProducts = _context.Products
                .OrderByDescending(p => p.ProductID)
                .Take(12)
                .ToList();
            var discountProducts = _context.Products
                .Where(p => _context.ProductDiscounts.Any(pd => pd.ProductID == p.ProductID))
                .Take(12)
                .ToList();
            var giftProducts = _context.Products
                .Where(p => !newProducts.Contains(p) && !discountProducts.Contains(p))
                .Take(12)
                .ToList();
            ViewBag.NewProducts = newProducts;
            ViewBag.DiscountProducts = discountProducts;
            ViewBag.GiftProducts = giftProducts;
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult CatalogPage()
        {
            var packages = _context.Products.Select(p => p.Package).Distinct().ToList();

            var flowerTypes = _context.FlowerType.Select(p => p.FlowerTypeName).Distinct().ToList();

            ViewBag.Packages = packages;
            ViewBag.FlowerTypes = flowerTypes;


            ViewBag.Categories = _context.FlowerCategories.ToList();
            ViewBag.Stores = _context.Stores.ToList();
            return View();
        }

        public IActionResult GetProducts(string search = "", int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null,
            int? storeId = null, string sort = "new", [FromForm] List<string> selectedSizes = null, 
            [FromForm] List<string> selectedPackages = null, List<string> selectedTypes=null)
        {
            var query = _context.Products.AsQueryable();

            // Применение фильтров
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (storeId.HasValue)
            {
                query = query.Where(p => _context.StoreFlowerStocks.Any(s => s.StoreID == storeId && s.FlowerTypeID == p.ProductID && s.Quantity > 0));
            }
            
            query = sort switch
            {
                "asc" => query.OrderBy(p => p.Price),
                "desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderByDescending(p => p.ProductID)
            };

            if (selectedSizes != null && selectedSizes.Any())
            {
                query = query.Where(p => selectedSizes.Contains(p.Size));
            }

            if (selectedPackages != null && selectedPackages.Any())
            {
                query = query.Where(p => selectedPackages.Contains(p.Package));
            }

            if (selectedTypes != null && selectedTypes.Any())
            {
                var productIdsWithSelectedTypes = _context.ProductContents
                    .Where(pc => _context.FlowerType.Any(ft => selectedTypes.Contains(ft.FlowerTypeName) && ft.FlowerTypeID == pc.FlowerTypeID))
                    .Select(pc => pc.ProductID)
                    .Distinct()
                    .ToList();

                query = query.Where(p => productIdsWithSelectedTypes.Contains(p.ProductID));
            }

            if (selectedTypes != null && selectedTypes.Any())
            {
                var productIdsWithSelectedTypes = _context.ProductContents
                    .Where(pc => _context.FlowerType.Any(ft => selectedTypes.Contains(ft.FlowerTypeName) && ft.FlowerTypeID == pc.FlowerTypeID))
                    .Select(pc => pc.ProductID)
                    .Distinct()
                    .ToList();

                query = query.Where(p => productIdsWithSelectedTypes.Contains(p.ProductID));
            }


            var products = query.Select(p => new
            {
                p.ProductID,
                p.ProductName,
                p.Price,
                p.Photo
            }).ToList();

            if (products.Count == 0)
            {
                return Json(new { message = "Нет товаров" });
            }

            return Json(products);
        }

        public IActionResult DeliveryPage()
        {
            return View();
        }

        public IActionResult ContactPage()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
