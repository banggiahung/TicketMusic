using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Data;
using TicketMusic.Data;
using TicketMusic.Models;
using TicketMusic.Models.ViewDashboard;
using TicketMusic.Services;

namespace TicketMusic.Areas.AdminTicket.Controllers
{
    [Area("AdminTicket")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminDashboardController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet("admin/dashboard")]
        public IActionResult Index()
        {
            var totalPrice = _context.Orders.Where(x => x.OrderStatus).Sum(x => x.TotalPrices);

            var user = _context.Users.Count();
            var products = _context.Products.Count();
            var category = _context.Categories.Count();

            var response = new ViewDashBorad
            {
                TotalPrice = totalPrice,
                TotalUser = user,
                TotalCategory = category,
                TotalTicket = products
            };
            return View(response);
        }



        [AllowAnonymous]
        [HttpPost("admin/upload-local-main")]
        public IActionResult UploadLocalMain(List<IFormFile> files, [FromServices] IUrlHelperFactory urlHelperFactory)
        {
            var filePaths = new List<string>();

            foreach (IFormFile photo in Request.Form.Files)
            {
                string sv = Path.Combine(_env.WebRootPath, "Upload", photo.FileName);
                using (var stream = new FileStream(sv, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                string relativePath = $"~/Upload/{photo.FileName}";
                string absolutePath = Url.Content(relativePath);

                filePaths.Add(absolutePath);
            }

            return Json(new { urls = filePaths });
        }

    }
}
