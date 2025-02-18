using Microsoft.AspNetCore.Mvc;
using NewFlowersShop.Models;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            var newProducts = _context.Products
                .Where(p => _context.StoreFlowerStocks.Any(pc => pc.FlowerTypeID == p.ProductID && pc.Quantity > 0))
                .OrderByDescending(p => p.ProductID)
                .Take(6)
                .ToList();

            var discountProducts = _context.Products
                .Where(p => _context.ProductDiscounts.Any(pd => pd.ProductID == p.ProductID) &&
                            _context.StoreFlowerStocks.Any(pc => pc.FlowerTypeID == p.ProductID && pc.Quantity > 0))
                .Take(12)
                .ToList();

            var giftProducts = _context.Products
                .Where(p => p.CategoryID == 10 && !newProducts.Contains(p) && !discountProducts.Contains(p) &&
                            _context.StoreFlowerStocks.Any(pc => pc.FlowerTypeID == p.ProductID && pc.Quantity > 0))
                .Take(6)
                .ToList();

            var SelectedBackground = _context.MainPage.FirstOrDefault(b => b.Used == "Да");
            ViewBag.SelectedBackground = SelectedBackground;

            ViewBag.BackgroundText = SelectedBackground?.BackgroundText;
            ViewBag.BackgroundTextColor = SelectedBackground?.BackgroundTextColor;

            if (SelectedBackground?.ButtonText != "error")
            {
                ViewBag.ButtonColor = SelectedBackground?.ButtonColor;
                ViewBag.ButtonTextColor = SelectedBackground?.ButtonTextColor;
                ViewBag.ButtonText = SelectedBackground?.ButtonText;
            }
            else
            {
                ViewBag.ButtonText = null;
            }
            ViewBag.Position = SelectedBackground?.Position;

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
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
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
                InStock = _context.StoreFlowerStocks.Any(pc => pc.FlowerTypeID == p.ProductID && pc.Quantity > 0)
            }).ToList();


            if (products.Count == 0)
            {
                return Json(new { message = "Нет товаров" });
            }

            return Json(products);
        }

        public IActionResult DeliveryPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            return View();
        }

        public IActionResult ContactPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
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
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            bool isAuthenticated = !string.IsNullOrEmpty(userRole);
            return Json(new { isAuthenticated, role = userRole });
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
            var userRole = HttpContext.Session.GetString("UserRole");
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
            ViewBag.UserRole = userRole;
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
                var role = _context.Roles.FirstOrDefault(r => r.RoleID == employee.RoleID)?.RoleName;
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
            ViewData["UserRole"] = HttpContext.Session.GetString("UserRole");

            return Json(new { success = false, message = "Неверный логин или пароль" });
        }

        [Route("Home/ProductPage/{productID:int}")]
        public IActionResult ProductPage(int productID)
        {
            var userRole = HttpContext.Session.GetString("UserRole"); ViewBag.UserRole = userRole;

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
        .Any(pc => pc.ProductID == productID && pc.FlowerTypeID == ft.FlowerTypeID))
    .Select(ft => ft.FlowerTypeName)
    .ToList();


            ViewData["FlowerTypes"] = string.Join(", ", flowerTypeNames);
            var storeIds = _context.StoreFlowerStocks
     .Where(sfs => _context.ProductContents.Any(pc => pc.ProductID == productID && sfs.FlowerTypeID == pc.ProductID) ||
                   !_context.ProductContents.Any(pc => pc.ProductID == productID) && sfs.FlowerTypeID == productID)
     .Select(sfs => sfs.StoreID)
     .Distinct()
     .ToList();

            var storeAddresses = _context.Stores
                .Where(store => storeIds.Contains(store.StoreID))
                .Select(store => store.Address)
                .ToList();

            ViewData["Stores"] = storeAddresses;


            ViewBag.FlowerCategories = categoryName;

            var productContents = _context.StoreFlowerStocks
    .Where(pc => pc.FlowerTypeID == productID)
    .ToList();

            int maxQuantity = productContents.Any() ? productContents.Max(pc => pc.Quantity) : 0;

            var reviews = _context.Reviews
        .Where(r => r.ProductID == productID && r.StatusID == 12)
        .OrderByDescending(r => r.ReviewDate)
        .ToList();

            ViewBag.Reviews = reviews;

            ViewData["MaxQuantity"] = maxQuantity;

            return View(product);
        }

        public IActionResult BasketPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole"); ViewBag.UserRole = userRole;
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

            var productContents = _context.StoreFlowerStocks
                .FirstOrDefault(pc => pc.FlowerTypeID == id);

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

            var productContents = _context.StoreFlowerStocks
                .FirstOrDefault(pc => pc.FlowerTypeID == id);

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
        public IActionResult Checkout()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            if (!cart.Any())
            {
                return Json(new { success = false, message = "Корзина пуста" });
            }

            ViewBag.TotalAmount = cart.Sum(item => item.Price * item.Quantity);
            return Json(new { success = true, items = cart });
        }

        public IActionResult OrderHistoryPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole"); ViewBag.UserRole = userRole;
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

            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;

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
                                ProductID = p.ProductID,
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

        [Route("Home/ReviewsPage/{productID:int}")]
        public IActionResult ReviewsPage(int productId)
        {
            var userRole = HttpContext.Session.GetString("UserRole"); ViewBag.UserRole = userRole;
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

            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ReviewViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Photo = product.Photo
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitReview([FromBody] ReviewInputModel model)
        {
            var login = HttpContext.Session.GetString("Login");
            if (string.IsNullOrEmpty(login))
            {
                return Unauthorized();
            }

            var user = _context.Customers.FirstOrDefault(u => u.LoginCustomer == login);
            if (user == null)
            {
                return Unauthorized();
            }

            var review = new Reviews
            {
                CustomerID = user.CustomerID,
                ProductID = model.ProductId,
                ReviewText = model.ReviewText,
                Rating = model.Rating,
                StatusID = 10,
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetReviewableProducts(int orderId)
        {
            var login = HttpContext.Session.GetString("Login");
            if (string.IsNullOrEmpty(login))
            {
                return Unauthorized();
            }

            var user = _context.Customers.FirstOrDefault(u => u.LoginCustomer == login);
            if (user == null)
            {
                return Unauthorized();
            }

            var products = _context.OrderContents
             .Where(op => op.OrderID == orderId)
             .Include(op => op.Product)
             .Select(op => new
             {
                 op.Product.ProductID,
                 op.Product.ProductName,
                 op.Product.Photo
             })
             .ToList();

            var reviewableProducts = products.Where(p =>
                !_context.Reviews.Any(r => r.CustomerID == user.CustomerID && r.ProductID == p.ProductID))
                .ToList();

            return Json(reviewableProducts);
        }

        public IActionResult OrderPlacementPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole"); ViewBag.UserRole = userRole;
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            ViewBag.TotalAmount = cart.Sum(item => item.Price * item.Quantity);
            ViewBag.Stores = _context.Stores.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult ProcessOrder()
        {
            var login = HttpContext.Session.GetString("Login");
            var user = _context.Customers.FirstOrDefault(u => u.LoginCustomer == login);

            var deliveryMethod = Request.Form["deliveryMethod"];
            var deliveryDate = Request.Form["courierDate"];
            var deliveryTime = Request.Form["courierTime"];
            var paymentMethod = Request.Form["paymentMethod"];
            var deliveryAddress = Request.Form["deliveryAddress"].FirstOrDefault() ?? "";
            var fullName = string.IsNullOrEmpty(Request.Form["NameCust"].ToString())
                ? user?.FirstName
                : Request.Form["NameCust"].ToString();
            var phone = string.IsNullOrEmpty(Request.Form["recipientPhone"].ToString())
                ? user?.PhoneNumber
                : Request.Form["recipientPhone"].ToString();



            if (string.IsNullOrEmpty(deliveryMethod) || string.IsNullOrEmpty(paymentMethod))
            {
                return Json(new { success = false, message = "Заполните все обязательные поля." });
            }

            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) : new List<CartItem>();

            if (!cart.Any())
            {
                return Json(new { success = false, message = "Корзина пуста." });
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var delivery = new Deliveries
                    {
                        DeliveryMethod = deliveryMethod,
                        DeliveryAddress = deliveryAddress,
                        DeliveryDate = DateTime.Parse(deliveryDate),
                        DeliveryTime = deliveryTime,
                        DeliveryName = fullName,
                        DeliveryPhone = phone,
                        DeliveryPayment = paymentMethod
                    };

                    _context.Deliveries.Add(delivery);
                    _context.SaveChanges();

                    var order = new Orders
                    {
                        CustomerID = user.CustomerID,
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                        StatusID = 14,
                        DeliveryID = delivery.DeliveryID
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    foreach (var item in cart)
                    {
                        var productContent = _context.StoreFlowerStocks.FirstOrDefault(pc => pc.FlowerTypeID == item.Id);

                        if (productContent == null || productContent.Quantity < item.Quantity)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = "Некоторых товаров нет в наличии." });
                        }

                        var orderContent = new OrderContents
                        {
                            OrderID = order.OrderID,
                            ProductID = item.Id,
                            Quantity = item.Quantity,
                            Price = item.Price
                        };

                        _context.OrderContents.Add(orderContent);

                        productContent.Quantity -= item.Quantity;
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    HttpContext.Session.Remove("Cart");

                    return Json(new { success = true, message = "Заказ успешно оформлен.", orderId = order.OrderID });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Ошибка при оформлении заказа: " + ex.Message });
                }
            }
        }

        public IActionResult productManagementPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole"); ViewBag.UserRole = userRole;
            var productStocks = _context.StoreFlowerStocks
                .Join(
                    _context.Products,
                    stock => stock.FlowerTypeID,
                    product => product.ProductID,
                    (stock, product) => new ProductStock
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        Quantity = stock.Quantity,
                        StoreID = stock.StoreID

                    }).Distinct()
                .ToList();

            var query = _context.Products.AsQueryable();
            var newProducts = query.Select(p => new
            {
                p.ProductID,
                p.ProductName,
                p.Price,
                p.Photo,
                InStock = _context.StoreFlowerStocks.Any(pc => pc.FlowerTypeID == p.ProductID && pc.Quantity > 0)
            }).ToList();
            ViewBag.Products = newProducts;


            ViewBag.ProductStocks = productStocks;
            var packages = _context.Products.Select(p => p.Package).Distinct().ToList();

            var flowerTypes = _context.FlowerType.ToList();

            ViewBag.Packages = packages;
            ViewBag.FlowerTypes = flowerTypes;


            ViewBag.Categories = _context.FlowerCategories.ToList();
            ViewBag.Stores = _context.Stores.ToList();

            return View();
        }

        private string ConvertImageToBase64(IFormFile photo)
        {
            if (photo == null)
            {
                Console.WriteLine("Фото не передано в метод ConvertImageToBase64.");
                return "";
            }

            if (photo.Length == 0)
            {
                Console.WriteLine("Фото передано, но пустое.");
                return "";
            }

            using (var ms = new MemoryStream())
            {
                photo.CopyTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult productAdd(IFormFile photo, int categoryID, string productName, string description,
      string careDescription, decimal price, string size, string package,
      List<int> selectedFlowerTypes, List<int> selectedStores, int quantity)
        {
            if (selectedStores == null || selectedFlowerTypes == null)
            {
                TempData["ErrorMessage"] = "Не выбраны магазины или типы цветов";
                return RedirectToAction("productManagementPage");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(productName) || price <= 0 || categoryID <= 0)
                {
                    TempData["ErrorMessage"] = "Заполните все обязательные поля!";
                    return RedirectToAction("productManagementPage");
                }

                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    string base64Photo = ConvertImageToBase64(photo);
                    var newProduct = new Products
                    {
                        CategoryID = categoryID,
                        ProductName = productName,
                        DescriptionProduct = description,
                        CareDescription = careDescription,
                        Price = price,
                        Size = size,
                        Package = package,
                        StatusID = 1,
                        Photo = base64Photo
                    };

                    _context.Products.Add(newProduct);
                    _context.SaveChanges();

                    if (selectedStores != null && selectedStores.Count > 0)
                    {
                        foreach (var storeID in selectedStores)
                        {
                            var newStock = new StoreFlowerStocks
                            {
                                StoreID = storeID,
                                FlowerTypeID = newProduct.ProductID,
                                Quantity = quantity
                            };
                            _context.StoreFlowerStocks.Add(newStock);
                        }
                    }

                    if (selectedFlowerTypes != null && selectedFlowerTypes.Count > 0)
                    {
                        foreach (var flowerTypeID in selectedFlowerTypes)
                        {
                            _context.ProductContents.Add(new ProductContents
                            {
                                ProductID = newProduct.ProductID,
                                FlowerTypeID = flowerTypeID
                            });
                        }
                    }

                    _context.SaveChanges();

                    transaction.Commit();
                    return RedirectToAction("productManagementPage");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Ошибка при сохранении!";
                    return RedirectToAction("productManagementPage");
                }
            }
        }

        [HttpPost]
        public IActionResult RequestRestock(int productId, int quantity, int storeID)
        {
            var login = HttpContext.Session.GetString("Login");
            var user = _context.Employees.FirstOrDefault(u => u.LoginEmployee == login);

            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                return BadRequest("Товар не найден.");
            }

            var restockRequest = new RestockRequests
            {
                ProductID = productId,
                Quantity = quantity,
                RequestDate = DateTime.Now,
                StatusID = 18,
                EmployeeID = user.EmployeeID,
                StoreID = storeID
            };
            _context.RestockRequests.Add(restockRequest);
            _context.SaveChanges();

            return Ok("Запрос на пополнение отправлен.");
        }

        [HttpPost]
        public IActionResult WriteOff(int productId, int quantity, int storeID)
        {
            var stock = _context.StoreFlowerStocks
                                .FirstOrDefault(s => s.FlowerTypeID == productId && s.StoreID == storeID);
            var prod = _context.Products.FirstOrDefault(p => p.ProductID == productId);

            if (stock == null)
            {
                return BadRequest("Нет товара для списания.");
            }
            else if (stock.Quantity < quantity)
            {
                stock.Quantity = 0;
                prod.StatusID = 2;
            } else
            {
                stock.Quantity -= quantity;
            }

            _context.Products.Update(prod);
            _context.StoreFlowerStocks.Update(stock);
            _context.SaveChanges();

            return Ok("Списание выполнено.");
        }

        public IActionResult OrderManagementPage()
        {
            var ordersQuery = _context.Orders
                .Where(o => o.StatusID == 14);

            var orders = ordersQuery
                .OrderBy(o => o.OrderDate)
                .Select(o => new OrderViewModel2
                {
                    OrderID = o.OrderID,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    StatusID = o.StatusID,
                    DeliveryID = o.DeliveryID,
                    Delivery = _context.Deliveries
                        .Where(d => d.DeliveryID == o.DeliveryID)
                        .Select(d => new OrderDelivery2
                        {
                            DeliveryDate = d.DeliveryDate,
                            DeliveryTime = d.DeliveryTime,
                            DeliveryAddress = d.DeliveryAddress,
                            DeliveryMethod = d.DeliveryMethod
                        }).FirstOrDefault(),
                    Products = _context.OrderContents
                        .Where(oc => oc.OrderID == o.OrderID)
                        .Select(oc => new Product2
                        {
                            Quantity = oc.Quantity,
                            Price = oc.Product.Price,
                            ProductName = oc.Product.ProductName,
                            ProductImage = oc.Product.Photo
                        }).ToList()
                })
                .ToList();

            ViewBag.Orders = orders;
            ViewBag.Stores = _context.Stores.ToList();

            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            return View();
        }

        [HttpPost]
        public IActionResult OrderManagementPageV2(int? statusId)
        {
            if (statusId == null)
            {
                ViewBag.OrdersStatus = null;
                ViewBag.SelectedStatus = statusId;
            }
            else if (statusId == 14)
            {
                ViewBag.OrdersStatus = null;
                ViewBag.SelectedStatus = 14;
            }
            else
            {
                var ordersQuery = _context.Orders.Where(o => o.StatusID == statusId);
                var ordersList = ordersQuery
                    .OrderBy(o => o.OrderDate)
                    .Select(o => new OrderViewModel2
                    {
                        OrderID = o.OrderID,
                        TotalAmount = o.TotalAmount,
                        OrderDate = o.OrderDate,
                        StatusID = o.StatusID,
                        DeliveryID = o.DeliveryID,
                        Delivery = _context.Deliveries
    .Where(d => d.DeliveryID == o.DeliveryID)
    .Select(d => new OrderDelivery2
    {
        DeliveryDate = d.DeliveryDate,
        DeliveryTime = d.DeliveryTime ?? "Не указано",
        DeliveryAddress = d.DeliveryAddress ?? "Адрес не указан",
        DeliveryMethod = d.DeliveryMethod ?? "Не указано"
    }).FirstOrDefault() ?? new OrderDelivery2(),
                        Products = _context.OrderContents
    .Where(oc => oc.OrderID == o.OrderID)
    .Select(oc => new Product2
    {
        Quantity = oc.Quantity,
        Price = oc.Product.Price,
        ProductName = oc.Product.ProductName,
        ProductImage = oc.Product.Photo
    }).ToList() ?? new List<Product2>()
                    })
                    .ToList();

                ViewBag.OrdersStatus = ordersList;
                ViewBag.SelectedStatus = 15;
                return Json(new { orders = ordersList });
            }
            return View();
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, int status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Заказ не найден" });
            }

            order.StatusID = status;

            if (status == 1018)
            {
                var orderContents = _context.OrderContents
                    .Where(oc => oc.OrderID == orderId)
                    .ToList();

                var delivery = _context.Deliveries.FirstOrDefault(d => d.DeliveryID == order.DeliveryID);
                if (delivery == null)
                {
                    return Json(new { success = false, message = "Доставка не найдена" });
                }

                string storeAddress = null;

                if (delivery.DeliveryMethod == "Самовывоз")
                {
                    var store = _context.Stores.FirstOrDefault(s => s.Address == delivery.DeliveryAddress);
                    if (store != null)
                    {
                        storeAddress = store.Address;
                    }
                }

                foreach (var orderContent in orderContents)
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductID == orderContent.ProductID);
                    if (product == null) continue;

                    var storeStock = _context.StoreFlowerStocks
                        .FirstOrDefault(sfs => sfs.FlowerTypeID == product.ProductID &&
                                               (storeAddress == null || sfs.Store.Address == storeAddress));

                    if (storeStock == null)
                    {
                        storeStock = _context.StoreFlowerStocks
                            .FirstOrDefault(sfs => sfs.FlowerTypeID == product.ProductID);
                    }

                    if (storeStock != null)
                    {
                        storeStock.Quantity += orderContent.Quantity;
                    }
                }

                _context.SaveChanges();
                return Json(new { success = true, message = "Заказ отменен, товары возвращены на склад" });
            }
            else
            {

                _context.SaveChanges();
                return Json(new { success = true, message = "Заказ передан курьеру" });
            }
        }


        [HttpPost]
        public IActionResult UpdateOrderStatus2(int orderId, int status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Заказ не найден" });
            }

            order.StatusID = status;

            _context.SaveChanges();
            return Json(new { success = true, message = "Заказ в сборке" });
        }

        public IActionResult GetOrderDetails(int orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderID == orderId)
                .Select(o => new OrderViewModel2
                {
                    OrderID = o.OrderID,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    StatusID = o.StatusID,
                    DeliveryID = o.DeliveryID,
                    Delivery = _context.Deliveries
                        .Where(d => d.DeliveryID == o.DeliveryID)
                        .Select(d => new OrderDelivery2
                        {
                            DeliveryDate = d.DeliveryDate,
                            DeliveryTime = d.DeliveryTime,
                            DeliveryAddress = d.DeliveryAddress,
                            DeliveryMethod = d.DeliveryMethod
                        }).FirstOrDefault(),
                    Products = _context.OrderContents
                        .Where(oc => oc.OrderID == o.OrderID)
                        .Select(oc => new Product2
                        {
                            Quantity = oc.Quantity,
                            Price = oc.Product.Price,
                            ProductName = oc.Product.ProductName,
                            ProductImage = oc.Product.Photo
                        }).ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return Json(new { success = false, message = "Заказ не найден" });
            }

            return Json(new { success = true, order });
        }

        private string ConvertImageToBase(IFormFile photo)
        {
            if (photo == null || photo.Length == 0) return "";

            using (var ms = new MemoryStream())
            {
                photo.CopyTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        [HttpPost]
        public IActionResult AddBackground(IFormFile photo, string BackgroundText, string BackgroundTextColor,
                                   string ButtonText, string ButtonColor, string ButtonTextColor,
                                   string Position)
        {
            var login = HttpContext.Session.GetString("Login");
            var user = _context.Employees.FirstOrDefault(u => u.LoginEmployee == login);
            var userRole = HttpContext.Session.GetString("UserRole");
            if (photo == null || photo.Length == 0)
            {
                return Json(new { success = false, message = "Файл изображения обязателен." });
            }

            if (ButtonText == null)
            {
                ButtonText = "error";
            }

            string base64Image = ConvertImageToBase(photo);
            var newBackground = new MainPage
            {
                BackgroundText = BackgroundText,
                BackgroundTextColor = BackgroundTextColor,
                ButtonText = ButtonText,
                ButtonColor = ButtonColor,
                ButtonTextColor = ButtonTextColor,
                Position = Position,
                Used = "Нет",
                UserId = user.EmployeeID,
                Photo = base64Image
            };

            _context.MainPage.Add(newBackground);
            _context.SaveChanges();

            return Json(new { success = true, data = newBackground });
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var backgrounds = _context.MainPage.ToList();
            return Json(backgrounds);
        }

        [HttpPost]
        public IActionResult SetBackgroundUsed(int backgroundId)
        {
            var background = _context.MainPage.FirstOrDefault(x => x.BackgroundId == backgroundId);
            if (background != null)
            {
                if (background.Used == "Нет")
                {
                    var otherBackgrounds = _context.MainPage.Where(x => x.BackgroundId != backgroundId).ToList();
                    foreach (var bg in otherBackgrounds)
                    {
                        bg.Used = "Нет";
                    }
                    background.Used = "Да";
                }
                else
                {
                    background.Used = "Нет";
                }

                _context.SaveChanges();
            }

            return Json(new { success = true, used = background?.Used });
        }


        public IActionResult homePageManagementPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            var backgrounds = _context.MainPage.ToList();
            var SelectedBackground = _context.MainPage.FirstOrDefault(b => b.Used == "Да");
            ViewBag.SelectedBackground = SelectedBackground;
            ViewBag.backgrounds = backgrounds;
            return View(backgrounds);
        }

        public IActionResult managingReviewsPage()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            var reviewsList = _context.Reviews.Where(r => r.StatusID == 11).ToList();
            ViewBag.reviewsList = reviewsList;

            return View();
        }

        [HttpPost]
        public IActionResult DeleteReview(int reviewId)
        {
            var review = _context.Reviews.Find(reviewId);
            if (review != null)
            {
                review.StatusID = 13; 
                _context.SaveChanges();
                return Json(new { success = true, message = "Отзыв успешно удален" });
            }
            return Json(new { success = false, message = "Отзыв не найден" });
        }

        [HttpPost]
        public IActionResult PostReview(int reviewId)
        {
            var review = _context.Reviews.Find(reviewId);
            if (review != null)
            {
                review.StatusID = 12;
                _context.SaveChanges();
                return Json(new { success = true, message = "Отзыв успешно опубликован" });
            }
            return Json(new { success = false, message = "Отзыв не найден" });
        }

        public IActionResult CourierOrderPage() 
        {
            var ordersQuery = _context.Orders
                .Where(o => o.StatusID == 16 && _context.Deliveries.Any(d => d.DeliveryID == o.DeliveryID && (d.DeliveryMethod == "Курьер" || d.DeliveryMethod == "Курьером")))
        .OrderBy(o => o.OrderDate);

            var orders = ordersQuery
                .OrderBy(o => o.OrderDate)
                .Select(o => new OrderViewModel3
                {
                    OrderID = o.OrderID,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    StatusID = o.StatusID,
                    DeliveryID = o.DeliveryID,
                    Delivery = _context.Deliveries
                        .Where(d => d.DeliveryID == o.DeliveryID)
                        .Select(d => new OrderDelivery3
                        {
                            DeliveryDate = d.DeliveryDate,
                            DeliveryTime = d.DeliveryTime,
                            DeliveryAddress = d.DeliveryAddress,
                            DeliveryName = d.DeliveryName,
                            DeliveryPhone = d.DeliveryPhone,
                            DeliveryMethod = d.DeliveryMethod
                        }).FirstOrDefault(),
                    Products = _context.OrderContents
                        .Where(oc => oc.OrderID == o.OrderID)
                        .Select(oc => new Product2
                        {
                            Quantity = oc.Quantity,
                            Price = oc.Product.Price,
                            ProductName = oc.Product.ProductName,
                            ProductImage = oc.Product.Photo
                        }).ToList()
                })
                .ToList();

            ViewBag.Orders = orders;

            return View();
        }

        public IActionResult GetOrderDetails2(int orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderID == orderId)
                .Select(o => new OrderViewModel3
                {
                    OrderID = o.OrderID,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    StatusID = o.StatusID,
                    DeliveryID = o.DeliveryID,
                    Delivery = _context.Deliveries
                        .Where(d => d.DeliveryID == o.DeliveryID)
                        .Select(d => new OrderDelivery3
                        {
                            DeliveryDate = d.DeliveryDate,
                            DeliveryTime = d.DeliveryTime,
                            DeliveryAddress = d.DeliveryAddress,
                            DeliveryName = d.DeliveryName,
                            DeliveryPhone = d.DeliveryPhone,
                            DeliveryMethod = d.DeliveryMethod
                        }).FirstOrDefault(),
                    Products = _context.OrderContents
                        .Where(oc => oc.OrderID == o.OrderID)
                        .Select(oc => new Product2
                        {
                            Quantity = oc.Quantity,
                            Price = oc.Product.Price,
                            ProductName = oc.Product.ProductName,
                            ProductImage = oc.Product.Photo
                        }).ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return Json(new { success = false, message = "Заказ не найден!" });
            }

            return Json(new { success = true, order });
        }

        [HttpPost]
        public IActionResult UpdateOrderStatusC(int orderId, int status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Заказ не доставлен!" });
            }

            order.StatusID = status;

            _context.SaveChanges();
            return Json(new { success = true, message = "Заказ доставлен!" });
        }

























        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
