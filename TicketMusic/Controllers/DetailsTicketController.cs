using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketMusic.Data;
using TicketMusic.Models;
using TicketMusic.Models.DetailsProductsView;
using TicketMusic.Services;

namespace TicketMusic.Controllers
{
    public class DetailsTicketController : Controller
    {
        private readonly ILogger<DetailsTicketController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _iConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommon _icommon;
        public DetailsTicketController(ILogger<DetailsTicketController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IConfiguration iConfiguration, ICommon icommon)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _iConfiguration = iConfiguration;
            _userManager = userManager;
            _icommon = icommon;
        }
        [HttpGet("event/{slug}")]
        public IActionResult DetailsProducts(string slug)
        {
            var user = new ApplicationUser();
            if (User.Identity.IsAuthenticated)
            {
                user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                ViewBag.userName = User.Identity.Name;
                ViewBag.NameUser = user.FullName;

            }
            var details = _context.Products
                .Where(x => x.Slug == slug)
                .Include(x => x.Categories)
                .Include(x => x.ProductVariants)
                .FirstOrDefault();
          
            if (details == null)
            {
                return NotFound();
            }
            var listProducts = _context.Products
              .Where(x => x.CategoryID == details.CategoryID && x.IDProduct != details.IDProduct)
              .Take(8)
              .Include(x => x.Categories)
              .Include(x => x.ProductVariants)
              .ToList();
            details.ViewCount++;
            _context.SaveChanges();
            var response = new DetailsProductView
            {
                Products = details,
                ProductsList = listProducts,
                User = user
            };
            return View(response);
        }
    }
}
