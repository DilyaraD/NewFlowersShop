using Microsoft.AspNetCore.Mvc;
using NewFlowersShop.Models;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;

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
                .Where(p => _context.ProductContents.Any(pc => pc.ProductID == p.ProductID && pc.Quantity > 0))
                .OrderByDescending(p => p.ProductID)
                .Take(12)
                .ToList();

            var discountProducts = _context.Products
                .Where(p => _context.ProductDiscounts.Any(pd => pd.ProductID == p.ProductID) &&
                            _context.ProductContents.Any(pc => pc.ProductID == p.ProductID && pc.Quantity > 0))
                .Take(12)
                .ToList();

            var giftProducts = _context.Products
                .Where(p => !newProducts.Contains(p) && !discountProducts.Contains(p) &&
                            _context.ProductContents.Any(pc => pc.ProductID == p.ProductID && pc.Quantity > 0))
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
            [FromForm] List<string> selectedPackages = null, List<string> selectedTypes = null)
        {
            var query = _context.Products.AsQueryable();

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
                p.Photo,
                InStock = _context.ProductContents.Any(pc => pc.ProductID == p.ProductID && pc.Quantity > 0)
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
         
        private byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        [HttpGet]
        public JsonResult IsAuthenticated()
        {
            bool isAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole"));
            return Json(new { isAuthenticated });
        }

        public IActionResult DataPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");
            var login = HttpContext.Session.GetString("Login");
            var fullName = HttpContext.Session.GetString("FullName");
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
            var password = HttpContext.Session.GetString("Password");

            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserRole = userRole;
            ViewBag.UserName = userName;
            ViewBag.Login = login;
            ViewBag.FullName = fullName;
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.Password = password;

            return View();
        }

        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var login = HttpContext.Session.GetString("Login");
            if (string.IsNullOrEmpty(login))
            {
                return RedirectToAction("Index", "Home");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.LoginCustomer == login);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }

            var employee = _context.Employees.FirstOrDefault(e => e.LoginEmployee == login);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index", "Home");
        }

        public IActionResult EditDataPage()
        {
            var login = HttpContext.Session.GetString("Login");
            var firstName = HttpContext.Session.GetString("FirstName");
            var lastName = HttpContext.Session.GetString("LastName");
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
            var password = HttpContext.Session.GetString("Password");

            if (string.IsNullOrEmpty(login))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Login = login;
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.Password = password;

            return View();
        }

        [HttpPost]
        public IActionResult SaveEdit(string login, string firstName, string lastName, string phoneNumber, string password)
        {
            var loginUser = HttpContext.Session.GetString("Login");
            var CusUserId = HttpContext.Session.GetInt32("CusUserId");
            var EmUserId = HttpContext.Session.GetInt32("EmUserId");

            Customers customer = null;
            Employees employee = null;

            if (CusUserId.HasValue && CusUserId > 0)
            {
                customer = _context.Customers.FirstOrDefault(c => c.CustomerID == CusUserId && c.LoginCustomer == loginUser);
            }
            else if (EmUserId.HasValue && EmUserId > 0)
            {
                employee = _context.Employees.FirstOrDefault(e => e.EmployeeID == EmUserId && e.LoginEmployee == loginUser);
            }

            if (customer != null)
            {
                customer.LoginCustomer = login;
                customer.FirstName = firstName;
                customer.LastName = lastName;
                customer.PhoneNumber = phoneNumber;

                if (!string.IsNullOrEmpty(password))
                {
                    customer.PasswordCustomer = HashPassword(password);
                }

                _context.Customers.Update(customer);
                _context.SaveChanges();

                HttpContext.Session.SetString("Login", login);
                HttpContext.Session.SetString("FirstName", firstName);
                HttpContext.Session.SetString("LastName", lastName);
                HttpContext.Session.SetString("PhoneNumber", phoneNumber);
                HttpContext.Session.SetString("FullName", $"{firstName} {lastName}");
                HttpContext.Session.SetString("Password", password);



                return Json(new { success = true, message = "Данные успешно сохранены.", redirectUrl = Url.Action("DataPage", "Home") });
            }
            else if (employee != null)
            {
                employee.LoginEmployee = login;
                employee.FirstName = firstName;
                employee.LastName = lastName;
                employee.PhoneNumber = phoneNumber;

                if (!string.IsNullOrEmpty(password))
                {
                    employee.PasswordEmployee = HashPassword(password);
                }

                _context.Employees.Update(employee);
                _context.SaveChanges();
                HttpContext.Session.SetString("Login", login);
                HttpContext.Session.SetString("FirstName", firstName);
                HttpContext.Session.SetString("LastName", lastName);
                HttpContext.Session.SetString("PhoneNumber", phoneNumber);
                HttpContext.Session.SetString("FullName", $"{firstName} {lastName}");
                HttpContext.Session.SetString("Password", password);

                return Json(new { success = true, message = "Данные успешно сохранены.", redirectUrl = Url.Action("DataPage", "Home") });
            }

            return Json(new { success = false, message = "Пользователь не найден." });
        }

        [HttpPost]
        public IActionResult Register(string login, string password, string firstName, string lastName, string phoneNumber)
        {
            if (_context.Customers.Any(c => c.LoginCustomer == login) || _context.Employees.Any(e => e.LoginEmployee == login))
            {
                return Json(new { success = false, message = "Логин уже занят" });
            }

            var newCustomer = new Customers
            {
                LoginCustomer = login,
                PasswordCustomer = HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Address = "-"
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("CusUserId", newCustomer.CustomerID);
            HttpContext.Session.SetInt32("EmUserId", 0);
            HttpContext.Session.SetString("UserRole", "Customer");
            HttpContext.Session.SetString("UserName", firstName);
            HttpContext.Session.SetString("Login", login);
            HttpContext.Session.SetString("FullName", $"{firstName} {lastName}");
            HttpContext.Session.SetString("PhoneNumber", phoneNumber);
            HttpContext.Session.SetString("Password", password);
            HttpContext.Session.SetString("LastName", lastName);
            HttpContext.Session.SetString("FirstName", firstName);

            return Json(new { success = true, redirectUrl = Url.Action("DataPage", "Home") });
        }



        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            byte[] hashedPassword = HashPassword(password);

            var customer = _context.Customers
                .Where(c => c.LoginCustomer == login && c.PasswordCustomer != null)
                .AsEnumerable()
                .FirstOrDefault(c => c.PasswordCustomer?.SequenceEqual(hashedPassword) == true);

            if (customer != null)
            {
                HttpContext.Session.SetInt32("CusUserId", customer.CustomerID);
                HttpContext.Session.SetInt32("EmUserId", 0);
                HttpContext.Session.SetString("UserRole", "Customer");
                HttpContext.Session.SetString("UserName", customer.FirstName);
                HttpContext.Session.SetString("Login", customer.LoginCustomer);
                HttpContext.Session.SetString("FullName", $"{customer.FirstName} {customer.LastName}");
                HttpContext.Session.SetString("PhoneNumber", customer.PhoneNumber);
                HttpContext.Session.SetString("Password", password);
                HttpContext.Session.SetString("LastName", customer.LastName);
                HttpContext.Session.SetString("FirstName", customer.FirstName);
                return RedirectToAction("DataPage", "Home");
            }

            var employee = _context.Employees
                .Where(e => e.LoginEmployee == login)
                .AsEnumerable()
                .FirstOrDefault(e => e.PasswordEmployee.SequenceEqual(hashedPassword)) ?? null;

            if (employee != null)
            {
                var role = _context.Roles.FirstOrDefault(r => r.RoleID == employee.RoleID)?.RoleName ?? "Employee";
                HttpContext.Session.SetInt32("EmUserId", employee.EmployeeID);
                HttpContext.Session.SetInt32("CusUserId", 0);
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserName", employee.FirstName);
                HttpContext.Session.SetString("Login", employee.LoginEmployee);
                HttpContext.Session.SetString("FullName", $"{employee.FirstName} {employee.LastName}");
                HttpContext.Session.SetString("PhoneNumber", employee.PhoneNumber);
                HttpContext.Session.SetString("Password", password);
                HttpContext.Session.SetString("LastName", employee.LastName);
                HttpContext.Session.SetString("FirstName", employee.FirstName);

                return RedirectToAction("DataPage", "Home");
            }

            return Json(new { success = false, message = "Неверный логин или пароль" });
        }

        [Route("Home/ProductPage/{productID:int}")]
        public IActionResult ProductPage(int productID)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductID == productID);
            if (product == null)
            {
                return Content($"Продукт не найден.");
            }

            var categoryName = _context.FlowerCategories
                .Where(c => c.CategoryID == product.CategoryID)
                .Select(c => c.CategoryName)
                .FirstOrDefault();

            var flowerTypeNames = _context.FlowerType
                .Where(ft => _context.ProductContents
                    .Any(pc => pc.ProductID == productID && pc.FlowerTypeID == ft.FlowerTypeID && pc.Quantity > 0))
                .Select(ft => ft.FlowerTypeName)
                .ToList();

            ViewData["FlowerTypes"] = string.Join(", ", flowerTypeNames);

            var storeIds = _context.StoreFlowerStocks
                .Where(sfs => _context.ProductContents
                    .Any(pc => pc.ProductID == productID && pc.FlowerTypeID == sfs.FlowerTypeID))
                .Select(sfs => sfs.StoreID)
                .Distinct()
                .ToList();

            var storeAddresses = _context.Stores
                .Where(store => storeIds.Contains(store.StoreID))
                .Select(store => store.Address)
                .ToList();

            ViewData["Stores"] = storeAddresses;

            ViewBag.FlowerCategories = categoryName;

            var productContents = _context.ProductContents
    .Where(pc => pc.ProductID == productID)
    .ToList();

int maxQuantity = productContents.Any() ? productContents.Max(pc => pc.Quantity) : 0;

            var reviews = _context.Reviews
        .Where(r => r.ProductID == productID)
        .OrderByDescending(r => r.ReviewDate)
        .ToList();

            ViewBag.Reviews = reviews;

            ViewData["MaxQuantity"] = maxQuantity;

            return View(product);
        }

        public IActionResult BasketPage()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, string name, decimal price, string photo, int quantity = 1)
        {
            var login = HttpContext.Session.GetString("Login");
            if (string.IsNullOrEmpty(login))
            {
                return Json(new { success = false, message = "Вы не авторизованы!" });
            }

            var productContents = _context.ProductContents
                .FirstOrDefault(pc => pc.ProductID == id);

            int maxQuantity = productContents != null ? productContents.Quantity : 0;

            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            var existingProduct = cart.FirstOrDefault(p => p.Id == id);
            if (existingProduct != null)
            {
                if (existingProduct.Quantity + quantity <= maxQuantity)
                {
                    existingProduct.Quantity += quantity;
                }
                else
                {
                    return Json(new { success = false, message = "Достигнуто максимальное количество на складе" });
                }
            }
            else
            {
                if (quantity > maxQuantity)
                {
                    return Json(new { success = false, message = "Достигнуто максимальное количество на складе" });
                }

                cart.Add(new CartItem { Id = id, Name = name, Price = price, Photo = photo, Quantity = quantity });
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));

            return Json(new { success = true, message = "Товар добавлен в корзину!" });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            var itemToRemove = cart.FirstOrDefault(item => item.Id == id);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity < 0)
            {
                return Json(new { success = false, message = "Количество не может быть отрицательным" });
            }

            var productContents = _context.ProductContents
                .FirstOrDefault(pc => pc.ProductID == id);

            int maxQuantity = productContents != null ? productContents.Quantity : 0;

            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            var itemToUpdate = cart.FirstOrDefault(item => item.Id == id);
            if (itemToUpdate != null)
            {
                if (quantity == 0)
                {
                    cart.Remove(itemToUpdate);
                }
                else if (quantity <= maxQuantity)
                {
                    itemToUpdate.Quantity = quantity;
                }
                else
                {
                    return Json(new { success = false, message = "Достигнуто максимальное количество на складе" });
                }

                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                return Json(new { success = true, message = "Количество товара изменено" });
            }

            return Json(new { success = false, message = "Товар не найден в корзине" });
        }

        [HttpPost]
        public IActionResult Checkout()////////////////////////////////////////////////////////////////////////&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&????????????????????????????????7
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            if (!cart.Any())
            {
                return Json(new { success = false, message = "Корзина пуста" });
            }

            return Json(new { success = true, items = cart });
        }

        public IActionResult OrderHistoryPage()
        {
            var login = HttpContext.Session.GetString("Login");
            if (string.IsNullOrEmpty(login))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = _context.Customers.FirstOrDefault(u => u.LoginCustomer == login);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var orders = _context.Orders
                .Where(o => o.CustomerID == user.CustomerID)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderViewModel
                {
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    StatusID = o.StatusID,
                    Products = _context.OrderContents
                        .Where(oc => oc.OrderID == o.OrderID)
                        .Join(_context.Products, oc => oc.ProductID, p => p.ProductID,
                            (oc, p) => new ProductViewModel
                            {
                                ProductName = p.ProductName,
                                Photo = p.Photo,
                                Quantity = oc.Quantity,
                                Price = oc.Price
                            })
                        .ToList()
                })
                .ToList();

            return View(orders);
        }

        public IActionResult DetailedOrderHistoryPage(int orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderID == orderId)
                .Select(o => new OrderViewModel
                {
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    StatusID = o.StatusID,
                    Delivery = _context.Deliveries.FirstOrDefault(d => d.DeliveryID == o.DeliveryID),
                    Products = _context.OrderContents
                        .Where(oc => oc.OrderID == o.OrderID)
                        .Join(_context.Products, oc => oc.ProductID, p => p.ProductID,
                            (oc, p) => new ProductViewModel
                            {
                                ProductName = p.ProductName,
                                Photo = p.Photo,
                                Quantity = oc.Quantity,
                                Price = oc.Price
                            })
                        .ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }










        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
