using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TicketMusic.Data;
using TicketMusic.Models;
using TicketMusic.Models.AccountVM;
using TicketMusic.Models.ProductsViewModel;
using TicketMusic.Models.ViewHome;
using TicketMusic.Services;

namespace TicketMusic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _iConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommon _icommon;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IConfiguration iConfiguration, ICommon icommon)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _iConfiguration = iConfiguration;
            _userManager = userManager;
            _icommon = icommon;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                ViewBag.userName = User.Identity.Name;
                ViewBag.NameUser = user.FullName;

            }

            var popular = _context.Products
                .Where(x=>x.IsPopular)
                .Include(x => x.Categories)
                .Include(x => x.ProductVariants)
                .OrderByDescending(x => x.CreateDate)
                .ToList();
            var listProducts = _context.Products
                 .Where(x => x.IsPopular == false)
                .Include(x => x.Categories)
                .Include(x => x.ProductVariants)
                .OrderByDescending(x => x.CreateDate)
                .ToList();

           
            var response = new ViewHomeIndex
            {
                ProductsPopular = popular,
                ProductsMost = listProducts
            };
            return View(response);
        }

        [HttpGet("search/ticket")]
        public IActionResult GetSearch(string search)
        {

            var products = (from i in _context.Products
                            where i.Name.Contains(search) 
                            select new ProductsCRUDViewModel
                            {
                                Name = i.Name,
                                Slug = i.Slug,
                                CreateDate = i.CreateDate
                            }).OrderByDescending(i => i.CreateDate);
            var result = products.Select(ToDictionaryWithNonNullProperties).ToList();
            return Ok(result);
        }
        public static IDictionary<string, object> ToDictionaryWithNonNullProperties<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var dictionary = new Dictionary<string, object>();
            foreach (var property in typeof(T).GetProperties())
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    dictionary.Add(property.Name, value);
                }
            }
            return dictionary;
        }
        public IActionResult Privacy()
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