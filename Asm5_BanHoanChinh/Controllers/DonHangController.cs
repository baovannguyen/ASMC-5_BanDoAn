
using Asm5_BanHoanChinh1.data;
using Asm5_BanHoanChinh1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Asm5_BanHoanChinh1.Controllers
{
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Checkout()
        {
            // Lấy thông tin giỏ hàng từ session
            List<Cart> cart = HttpContext.Session.GetJson<List<Cart>>("Cart");

            if (cart == null || cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Lấy MaNguoiDung từ session
         

            // Kiểm tra xem người dùng đã đăng nhập chưa
            //if (string.IsNullOrEmpty(userId))
            //{
            //    TempData["Message"] = "Vui lòng đăng nhập trước khi thanh toán!";
            //    return RedirectToAction("Login", "User"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
            //}
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Lỗi: Không thể xác định người dùng.";
                return RedirectToAction("Login", "User");
            }
            // Chuyển đổi userId thành kiểu int
         

            // Tạo hóa đơn mới
            DonHangModel donHang = new DonHangModel
            {
                KhachHangId = userId, // Sử dụng MaNguoiDung từ session
                NgayDat = DateTime.Now,
                TrangThai = "Đã thanh toán",
                TongTien = cart.Sum(c => c.Total)
            };

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // Lưu chi tiết đơn hàng
            foreach (var item in cart)
            {
                // Kiểm tra nếu sản phẩm là món ăn
                var monAn = await _context.MonAns.FindAsync((int)item.ProductId);
                if (monAn != null)
                {
                    ChiTietDonHangModel chiTiet = new ChiTietDonHangModel
                    {
                        DonHangId = donHang.Id,
                        MonAnId = (int)item.ProductId,
                        SoLuong = item.Quantity,
                        Gia = item.Price
                    };
                    _context.ChiTietDonHangs.Add(chiTiet);
                }
                else
                {
                    // Nếu sản phẩm không tồn tại trong cả MonAn 
                    TempData["Error"] = $"Sản phẩm với ID {item.ProductId} không tồn tại.";
                    return RedirectToAction("Index");
                }
            }
            await _context.SaveChangesAsync();
       

            // Xóa giỏ hàng khỏi session
            HttpContext.Session.Remove("Cart");

            TempData["Message"] = "Đặt hàng thành công!";
            return RedirectToAction("OrderStatus", new { id = donHang.Id });
        }




        public async Task<IActionResult> OrderStatus(int id)
        {
            // 4. Lấy danh sách loại món ăn
            var loaiList = _context.MonAns
                                   .Select(ma => ma.Loai)
                                   .Distinct()
                                   .ToList();

            // 5. Gán vào ViewBag
            ViewBag.LoaiList = loaiList;
            var donHang = await _context.DonHangs
       .Include(dh => dh.ChiTietDonHangs)
           .ThenInclude(ctdh => ctdh.MonAn)      
       .FirstOrDefaultAsync(dh => dh.Id == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        public async Task<IActionResult> PaidOrders()
        {
            // 4. Lấy danh sách loại món ăn
            var loaiList = _context.MonAns
                                   .Select(ma => ma.Loai)
                                   .Distinct()
                                   .ToList();

            // 5. Gán vào ViewBag
            ViewBag.LoaiList = loaiList; ; // Lấy danh sách từ database
          
            // Lấy userId từ session
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra xem người dùng đã đăng nhập chưa (userId sẽ là null nếu chưa đăng nhập)
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Chuyển đổi userId thành kiểu int
           

            // Truy vấn các đơn hàng đã thanh toán của người dùng hiện tại
            var paidOrders = await _context.DonHangs
                .Where(dh => dh.TrangThai == "Đã thanh toán" && dh.KhachHangId == userId)
                .OrderByDescending(dh => dh.NgayDat)
                .ToListAsync();

            return View(paidOrders);
        }


    }
}
