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
    public class AdminCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminCategoriesController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet("admin/categories")]
        public IActionResult Index()
        {
            var results = _context.Categories.OrderByDescending(x=>x.CategoryID).ToList();
            return View(results);
        }
        [HttpGet("admin/categories/edit/{Id}")]
        public IActionResult EditCategories(int Id)
        {
            var results = _context.Categories
                .FirstOrDefault(x => x.CategoryID == Id);
            if(results != null)
            {
              
                return View(results);

            }
            return NotFound();


        }
        [HttpPost("admin/categories/add")]
        public async Task<IActionResult> AddCategories(Categories model)
        {
            try
            {
                var existingProduct = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == model.Slug);
                if (existingProduct != null)
                {
                    return Ok(new { code = 400, message = "Trùng slug, vui lòng nhập lại" });

                }
                
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new { code = 200, message = "Thành công"});

            }
            catch (Exception ex)
            {
                return Ok(new {code = 400, message = ex.Message});
            }
           

        }
        [HttpPost("admin/categories/remove")]
        public async Task<IActionResult> DeleteCategory(int ID)
        {
            try
            {

                var existingProduct = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryID == ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                if (existingProduct.IsDefault)
                {
                    return Ok(new { code = 400, meesage = "Không xóa danh mục mặc định" });

                }

                List<Products> products =  _context.Products.Where(x => x.CategoryID == existingProduct.CategoryID).ToList();
                if (products.Any())
                {
                    var category = _context.Categories.FirstOrDefault(x => x.IsDefault);
                    if (category != null)
                    {
                        foreach(var product in products)
                        {
                            product.CategoryID = category.CategoryID;
                            _context.Products.Update(product);
                            _context.SaveChanges();

                        }
                    }
                }
                _context.Categories.Remove(existingProduct);
                await _context.SaveChangesAsync();
                return Ok(new {code = 200});

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("admin/categories/change-default")]
        public async Task<IActionResult> ChangeDefaultCategory(int ID)
        {
            try
            {

                var existingProduct = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryID == ID);
                if (existingProduct == null)
                {
                    return Ok(new { code = 400 });

                }
                if(existingProduct.IsDefault)
                {
                    return Ok(new { code = 400, meesage = "Đang là mặc định" });

                }
                var newCategory = await _context.Categories.FirstOrDefaultAsync(x => x.IsDefault);
                if (newCategory != null)
                {
                    newCategory.IsDefault = false;
                    existingProduct.IsDefault = true;
                    _context.Categories.Update(newCategory);
                    _context.Categories.Update(existingProduct);
                    await _context.SaveChangesAsync();
                    return Ok(new { code = 200 });

                }


                return Ok(new { code = 400, meesage="Có lỗi xảy ra" });

            }

            catch (Exception ex)
            {
                return Ok(new { code = 500 });
            }

        }


        [HttpPost("admin/category/update")]
        public async Task<IActionResult> UpdateProducts(Categories model)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(x=>x.CategoryID == model.CategoryID);
                if (category == null)
                {
                    return Ok(new { code = 400, message = "Lỗi" });

                }
                category.CategoryName = model.CategoryName;
                category.Slug = model.Slug;
                category.IsDefault = model.IsDefault;
               
                var existingProduct = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.CategoryID != category.CategoryID);
                if (existingProduct != null)
                {
                    return Ok(new { code = 400, message = "Trùng slug, vui lòng nhập lại" });

                }
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return Ok(new { code = 200, message = "Thành công" });

            }
            catch (Exception ex)
            {
                return Ok(new { code = 400, message = ex.Message });
            }


        }
    }
}
