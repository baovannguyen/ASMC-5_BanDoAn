using Asm5_BanHoanChinh1.data;
using Asm5_BanHoanChinh1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Asm5_BanHoanChinh1.Controllers
{
    [Authorize]
    public class ManagerHoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerHoaDonController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var orders = await _context.DonHangs
                .Where(dh => dh.KhachHangId == user.Id)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> DeleteOrders(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.DonHangs.FirstOrDefaultAsync(dh => dh.Id == id && dh.KhachHangId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            _context.DonHangs.Remove(order);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Xóa đơn hàng thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditOrders(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var donHang = await _context.DonHangs.FirstOrDefaultAsync(dh => dh.Id == id && dh.KhachHangId == user.Id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrders(int id, DonHangModel updatedDonHang)
        {
            var user = await _userManager.GetUserAsync(User);
            var donHang = await _context.DonHangs.FirstOrDefaultAsync(dh => dh.Id == id && dh.KhachHangId == user.Id);

            if (donHang == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                donHang.NgayDat = updatedDonHang.NgayDat;
                donHang.TrangThai = updatedDonHang.TrangThai;
                donHang.TongTien = updatedDonHang.TongTien;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(updatedDonHang);
        }
    }
}
