using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using TicketMusic.Data;
using TicketMusic.Models;
using TicketMusic.Models.ProductsViewModel;
using TicketMusic.Services;

namespace TicketMusic.Areas.AdminTicket.Controllers
{
    [Area("AdminTicket")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminOrdersController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet("admin/orders")]
        public IActionResult Index()
        {
            var results = _context.Orders
                .Include(x=>x.OrdersDetails)
                    .ThenInclude(x=>x.ProductVariants)
                   .ThenInclude(x => x.Products)
                .OrderByDescending(x=>x.CreateDate)
                .ToList();
            foreach (var order in results)
            {
                foreach (var orderDetail in order.OrdersDetails)
                {
                    // Xóa thuộc tính Description của đối tượng Products
                    orderDetail.ProductVariants.Products.Description = null;
                    orderDetail.ProductVariants.Products.ShortDescription = null;
                }
            }
            return View(results);
        }
        
    }
}
