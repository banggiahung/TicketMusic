using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TicketMusic.Data;
using TicketMusic.Models;
using TicketMusic.Models.ProductsViewModel;
using TicketMusic.Services;

namespace TicketMusic.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _iConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommon _icommon;
        private readonly IVnPayService _vnPayService;

        public OrdersController(IVnPayService vnPayService,ILogger<OrdersController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IConfiguration iConfiguration, ICommon icommon)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _iConfiguration = iConfiguration;
            _userManager = userManager;
            _icommon = icommon;
            _vnPayService = vnPayService;

        }

        [HttpPost("payment/create-payment-url")]
        public async Task<IActionResult> Index(PaymentInformationModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                if (user == null)
                {
                    return Ok(new { code = 400, meesage = "Kiểm tra lại thông tin đăng nhập" });
                }
                model.OrderCode = _icommon.GenerateRandomString(7);
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
                var createOrder = new Orders();
                createOrder.OrderCode = model.OrderCode;
                createOrder.UserID = user.Id;
                createOrder.Address = model.Address;
                createOrder.TotalPrices = model.Amount;
                createOrder.PhoneNumber = model.PhoneNumber;
                createOrder.Customer_Name = model.Name;
                createOrder.OrderStatus = false;
                createOrder.CreateDate = DateTime.Now;
                _context.Add(createOrder);
               await _context.SaveChangesAsync();

                List<ProductVariantsOrders> variantsList = JsonConvert.DeserializeObject<List<ProductVariantsOrders>>(model.ListProducts);

                if (variantsList.Any())
                {
                    foreach (var variant in variantsList)
                    {
                        var variantDetails = _context.ProductVariants.FirstOrDefault(x => x.ProductVariantsID == variant.productId);
                        var orderDetails = new OrdersDetails();
                        orderDetails.OrderID = createOrder.OrderID;
                        orderDetails.ProductsID = variant.productMainID;
                        orderDetails.ProductsVariants = variant.productId;
                        orderDetails.Quantity = variant.quantity;
                        orderDetails.ProductsPrice = variantDetails.PriceVariants;
                        _context.Add(orderDetails);
                        await _context.SaveChangesAsync();

                    }
                }

                return Ok(new { code = 200, url = url });
            }
            return Ok(new { code = 400, meesage = "Kiểm tra lại thông tin đăng nhập" });

        }
        [HttpGet("payment/succes")]
        public  IActionResult ActiveSuccess()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                ViewBag.userName = User.Identity.Name;
                ViewBag.NameUser = user.FullName;

            }
            ViewBag.OrderCode = TempData["OrderCode"] as string;
            return View();
        }  
        [HttpGet("payment/false")]
        public  IActionResult ActiveFalse()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                ViewBag.userName = User.Identity.Name;
                ViewBag.NameUser = user.FullName;

            }
            ViewBag.OrderCode = TempData["OrderCode"] as string;

            return View();
        }
        [HttpGet("payment/payment-callback")]
        public async Task<IActionResult> PaymentCallbackIPN(string vnp_Amount, string vnp_BankCode, string vnp_CardType, string vnp_OrderInfo, string vnp_PayDate, string vnp_ResponseCode, string vnp_SecureHash, string vnp_TmnCode, string vnp_TransactionNo, string vnp_TransactionStatus, string vnp_TxnRef)
        {
            try
            {
                ViewBag.userName = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                if (user == null)
                {
                    return Redirect("/payment/succes");
                }
                var rs = _context.Orders.FirstOrDefault(x => x.OrderCode.Trim() == vnp_OrderInfo.Trim() && x.OrderStatus == false && x.UserID == user.Id);

                var response = await _vnPayService.PaymentExecute(Request.Query);
                if (response.Success)
                {

                    if (rs != null)
                    {
                        var totalPrice = rs.TotalPrices * 100;

                        if (totalPrice.ToString() == vnp_Amount)
                        {
                            if (response.checkSignature == false)
                            {
                                TempData["OrderCode"] = rs.OrderCode;

                                List<OrdersDetails> orders = _context.OrdersDetails.Where(x=>x.OrderID == rs.OrderID).ToList();
                                if(orders.Count > 0)
                                {
                                    foreach(var order in orders)
                                    {
                                        _context.Remove(order);
                                        await _context.SaveChangesAsync();

                                    }
                                }
                                _context.Remove(rs);
                                await _context.SaveChangesAsync();
                                return Redirect("/payment/false");

                            }
                            if (rs.OrderStatus == false)
                            {
                                if (response.VnPayResponseCode == "00" && vnp_TransactionStatus == "00")
                                {
                                    TempData["OrderCode"] = rs.OrderCode;
                                    rs.OrderStatus = true;
                                    List<OrdersDetails> orders = _context.OrdersDetails.Where(x => x.OrderID == rs.OrderID).ToList();
                                    if (orders.Count > 0)
                                    {
                                        foreach (var order in orders)
                                        {
                                            var productVariant = _context.ProductVariants.FirstOrDefault(x => x.ProductVariantsID == order.ProductsVariants);
                                            if(productVariant.QuantityTicket - order.Quantity < 0)
                                            {
                                                order.Quantity = productVariant.QuantityTicket;
                                                productVariant.QuantityTicket = 0;
                                            }
                                            else
                                            {
                                                productVariant.QuantityTicket -= order.Quantity;

                                            }
                                            await _context.SaveChangesAsync();

                                        }
                                    }
                                    await _context.SaveChangesAsync();
                                    return Redirect("/payment/succes");

                                }
                                else if (response.VnPayResponseCode == "99")
                                {
                                    TempData["OrderCode"] = rs.OrderCode;

                                    List<OrdersDetails> orders = _context.OrdersDetails.Where(x => x.OrderID == rs.OrderID).ToList();
                                    if (orders.Count > 0)
                                    {
                                        foreach (var order in orders)
                                        {
                                            _context.Remove(order);
                                            await _context.SaveChangesAsync();

                                        }
                                    }
                                    _context.Remove(rs);
                                    await _context.SaveChangesAsync();
                                    return Redirect("/payment/false");
                                }



                            }
                        }

                        else
                        {
                            TempData["OrderCode"] = rs.OrderCode;

                            List<OrdersDetails> orders = _context.OrdersDetails.Where(x => x.OrderID == rs.OrderID).ToList();
                            if (orders.Count > 0)
                            {
                                foreach (var order in orders)
                                {
                                    _context.Remove(order);
                                    await _context.SaveChangesAsync();

                                }
                            }
                            _context.Remove(rs);
                            await _context.SaveChangesAsync();
                            return Redirect("/payment/false");
                        }


                    }

                   
                }
                return Redirect("/payment/false");

            }
            catch (Exception ex)
            {
                return Redirect("/payment/false");

            }
        }
    }
}
