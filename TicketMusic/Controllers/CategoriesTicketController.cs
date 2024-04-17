using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketMusic.Data;
using TicketMusic.Models;
using TicketMusic.Models.CategoryViewModel;
using TicketMusic.Models.DetailsProductsView;
using TicketMusic.Services;

namespace TicketMusic.Controllers
{
    public class CategoriesTicketController : Controller
    {
        private readonly ILogger<CategoriesTicketController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _iConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommon _icommon;
        public CategoriesTicketController(ILogger<CategoriesTicketController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IConfiguration iConfiguration, ICommon icommon)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _iConfiguration = iConfiguration;
            _userManager = userManager;
            _icommon = icommon;
        }
        [HttpGet("event/category/{slug}")]
        public IActionResult CategoryDetails(string slug)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                ViewBag.userName = User.Identity.Name;
                ViewBag.NameUser = user.FullName;

            }
            var category = _context.Categories
                .Where(x => x.Slug == slug)
                .FirstOrDefault();
          
            if (category == null)
            {
                return NotFound();
            }
            var listProducts = _context.Products
              .Where(x => x.CategoryID == category.CategoryID)
              .Take(8)
              .Include(x => x.Categories)
              .Include(x => x.ProductVariants)
              .ToList();
            var count = _context.Products.Where(x => x.CategoryID == category.CategoryID).Count();
            var response = new CategoryViewHome
            {
                Products = listProducts,
                Categories = category,
                CountEvent = count
            };
            return View(response);
        }


        [HttpGet("event/category/get-more")]

        public IActionResult LoadMore(int page, int pageSize, int CategoryId)
        {
            pageSize = 8;
            var skip = (page - 1) * pageSize;

            var listProducts = _context.Products
            .Where(x => x.CategoryID == CategoryId)
            .Skip(skip)
            .Take(pageSize)
            .Include(x => x.Categories)
            .Include(x => x.ProductVariants)
            .ToList();

            return PartialView("_CategoryListProducts", listProducts);
        }
    }
}
