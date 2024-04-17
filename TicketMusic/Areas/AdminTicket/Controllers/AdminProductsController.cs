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
    public class AdminProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICommon _iCommon;
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public AdminProductsController(ApplicationDbContext context, IConfiguration configuration, ICommon common, IWebHostEnvironment iHostingEnvironment, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _iCommon = common;
            _iHostingEnvironment = iHostingEnvironment;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet("admin/products")]
        public IActionResult Index()
        {
            var results = _context.Products.Include(x=>x.Categories)
                .Include(x => x.ProductVariants)
                .OrderByDescending(x=>x.IDProduct)
                .ToList();
            var listCategory = _context.Categories.ToList();

            var response = new ProductsViewAdmin
            {
                ProductsList = results,
                CategoriesList = listCategory
            };
            return View(response);
        }
        [HttpGet("admin/products/edit/{Id}")]
        public IActionResult EditProducts(int Id)
        {
            var results = _context.Products.Include(x => x.Categories)
                .Include(x => x.ProductVariants)
                .FirstOrDefault(x => x.IDProduct == Id);
            var listCategory = _context.Categories.ToList();
            if(results != null)
            {
                var response = new ProductsViewAdmin
                {
                    ProductsEdit = results,
                    CategoriesList = listCategory
                };
                return View(response);

            }
            return NotFound();


        }
        [HttpPost("admin/products/add")]
        public async Task<IActionResult> AddProducts(ProductsCRUDViewModel model)
        {
            try
            {
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Slug == model.Slug);
                if (existingProduct != null)
                {
                    return Ok(new { code = 400, message = "Trùng slug, vui lòng nhập lại" });

                }
                var products = new Products();
                List<ProductVariantsCRUD> variantsList = JsonConvert.DeserializeObject<List<ProductVariantsCRUD>>(model.Variants);
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;
                model.ViewCount = 1;
                if (model.PrPath != null)
                {
                    var PrPath = await _iCommon.UploadedFile(model.PrPath);
                    model.ImageEvent = "/Upload/" + PrPath;
                }
                if(model.CategoryID == 0 || model.CategoryID == null)
                {
                    var category = _context.Categories.FirstOrDefault(x => x.IsDefault);
                    if (category != null)
                    {
                        model.CategoryID = category.CategoryID;
                    }
                }
                products = model;
                _context.Products.Add(products);
                await _context.SaveChangesAsync();

                foreach (var item in variantsList)
                {
                    var variantItem = new ProductVariants();
                    variantItem.VariantsValue = item.variantsValue;
                    variantItem.PriceVariants = item.priceVariants;
                    variantItem.ProductID = products.IDProduct;
                    variantItem.QuantityTicket = item.quantityTicket;

                    _context.Add(variantItem);
                }
                await _context.SaveChangesAsync();
                return Ok(new { code = 200, message = "Thành công"});

            }
            catch (Exception ex)
            {
                return Ok(new {code = 400, message = ex.Message});
            }
           

        }
        [HttpPost("admin/products/remove")]
        public async Task<IActionResult> DeleteProducts(int ID)
        {
            try
            {

                var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.IDProduct == ID);
                if (existingProduct == null)
                {
                    return NotFound();

                }
                List<ProductVariants> variants = _context.ProductVariants.Where(i => i.ProductID == existingProduct.IDProduct).ToList();
                if (variants.Any())
                {
                    foreach(var variant in variants)
                    {
                        _context.ProductVariants.Remove(variant);

                    }

                }
                _context.Products.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(new {code = 200});

            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("admin/products/remove/variants")]
        public async Task<IActionResult> DeleteVariants(int ID)
        {
            try
            {

                var existingProduct = await _context.ProductVariants.FirstOrDefaultAsync(x => x.ProductVariantsID == ID);
                if (existingProduct == null)
                {
                    return Ok(new { code = 400 });

                }
               
                _context.ProductVariants.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(new { code = 200 });

            }

            catch (Exception ex)
            {
                return Ok(new { code = 500 });
            }

        }


        [HttpPost("admin/products/update")]
        public async Task<IActionResult> UpdateProducts(ProductsCRUDViewModel model)
        {
            try
            {
                var products = _context.Products.FirstOrDefault(x=>x.IDProduct == model.IDProduct);
                if (products == null)
                {
                    return Ok(new { code = 400, message = "Lỗi" });

                }
                List<ProductVariantsCRUDEdit> variantsList = JsonConvert.DeserializeObject<List<ProductVariantsCRUDEdit>>(model.Variants);
                model.UpdateDate = DateTime.Now;
                if (model.PrPath != null)
                {
                    var PrPath = await _iCommon.UploadedFile(model.PrPath);
                    model.ImageEvent = "/Upload/" + PrPath;
                }
                products.Name = model.Name;
                products.Slug = model.Slug;
                products.ShortDescription = model.ShortDescription;
                products.Description = model.Description;
                products.TimeEvent = model.TimeEvent;
                products.Location = model.Location;
                products.UpdateDate = model.UpdateDate;
                if (model.CategoryID == 0 || model.CategoryID == null)
                {
                    var category = _context.Categories.FirstOrDefault(x => x.IsDefault);
                    if (category != null)
                    {
                        products.CategoryID = category.CategoryID;
                    }
                }
                else
                {
                    products.CategoryID = model.CategoryID;

                }
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Slug == products.Slug && p.IDProduct != products.IDProduct);
                if (existingProduct != null)
                {
                    return Ok(new { code = 400, message = "Trùng slug, vui lòng nhập lại" });

                }
                _context.Products.Update(products);
                await _context.SaveChangesAsync();

                foreach (var item in variantsList)
                {
                    if(item.ProductVariantsID == 0)
                    {
                        var variantItem = new ProductVariants();
                        variantItem.VariantsValue = item.VariantsValue;
                        variantItem.PriceVariants = item.PriceVariants;
                        variantItem.ProductID = products.IDProduct;
                        variantItem.QuantityTicket = item.QuantityTicket;
                        _context.Add(variantItem);
                    }
                   
                }
                await _context.SaveChangesAsync();
                return Ok(new { code = 200, message = "Thành công" });

            }
            catch (Exception ex)
            {
                return Ok(new { code = 400, message = ex.Message });
            }


        }
        [HttpPost("admin/product/change-popular")]
        public async Task<IActionResult> ChangePopular(int ID)
        {
            try
            {

                var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.IDProduct == ID);
                if (existingProduct == null)
                {
                    return Ok(new { code = 400 });

                }
                existingProduct.IsPopular = !existingProduct.IsPopular;
                _context.Update(existingProduct);
                _context.SaveChanges();

                return Ok(new { code = 200, meesage = "Thành công" });

            }

            catch (Exception ex)
            {
                return Ok(new { code = 500 });
            }

        }
    }
}
