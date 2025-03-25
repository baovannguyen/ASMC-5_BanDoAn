
using Asm5_BanHoanChinh1.data;
using Microsoft.AspNetCore.Mvc;

using Asm5_BanHoanChinh1.Models;
using Asm5_BanHoanChinh1.Models.ViewModels;


namespace Asm5_BanHoanChinh1.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            // 4. Lấy danh sách loại món ăn
            var loaiList = _context.MonAns
                                   .Select(ma => ma.Loai)
                                   .Distinct()
                                   .ToList();

            // 5. Gán vào ViewBag
            ViewBag.LoaiList = loaiList;
            List<Cart> cartItems = HttpContext.Session.GetJson<List<Cart>>("Cart") ?? new List<Cart>();
            CartViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Total)
            };
            return View(cartVM);
        }
        public async Task<IActionResult> Add(int Id)
        {
            List<Cart> cart = HttpContext.Session.GetJson<List<Cart>>("Cart") ?? new List<Cart>();
            Cart cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            // Kiểm tra trong bảng MonAn
            MonAnModel product = await _context.MonAns.FindAsync(Id);
            if (product != null)
            {
                if (cartItem == null)
                {
                    cart.Add(new Cart(product));
                }
                else
                {
                    cartItem.Quantity += 1;
                }
            }
            else
            {
                
                return NotFound();
            }

            HttpContext.Session.SetJson("Cart", cart);
            return Redirect(Request.Headers["Referer"].ToString());
        }



        public async Task<IActionResult> Decrease(int Id)
        {
            List<Cart> cart = HttpContext.Session.GetJson<List<Cart>>("Cart");
            Cart cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }


            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            List<Cart> cart = HttpContext.Session.GetJson<List<Cart>>("Cart");
            Cart cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }


            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<Cart> cart = HttpContext.Session.GetJson<List<Cart>>("Cart");
            cart.RemoveAll(p => p.ProductId == Id);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }




    }
}
