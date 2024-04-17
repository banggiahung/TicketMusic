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
    public class AdminCustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminCustomerController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet("admin/customer")]
        public IActionResult Index()
        {
            var results = _context.Users.Where(x=>x.UserName != "ticketadmin").OrderByDescending(c=>c.CreateDate).ToList();
            return View(results);
        }
        [HttpPost("admin/customer/change-active")]
        public IActionResult ChangeActive(string userId) 
        {
            var user = _context.Users.FirstOrDefault(x=>x.Id == userId);
            if (user == null)
            {
                return Ok(new { code = 400, message = "Không tìm thấy user" });

            }
            user.IsActive = !user.IsActive;
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok(new { code = 200, message = "Thành công" });

        }

        [HttpPost("admin/customer/remove-user")]
        public IActionResult RemoveUser(string userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return Ok(new { code = 400, message = "Không tìm thấy user" });

            }
            List<Orders> order = _context.Orders.Where(x=>x.UserID == userId).ToList();
            if (order.Any())
            {
                foreach(var od in order)
                {
                    List<OrdersDetails> orderDetails = _context.OrdersDetails.Where(x=>x.OrderID == od.OrderID).ToList();
                    if (orderDetails.Any())
                    {
                        foreach(var detials in orderDetails)
                        {
                            _context.Remove(detials);
                            _context.SaveChanges();

                        }
                    }
                    _context.Remove(od);
                    _context.SaveChanges();

                }
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok(new { code = 200, message = "Thành công" });

        }

    }
}
