using Asm5_BanHoanChinh1.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asm5_BanHoanChinh1.Controllers
{
    public class MonAnController : Controller
    {
        private readonly ApplicationDbContext _context;

      
        public MonAnController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index(string search, string loai)
        {
            // 1. Lấy dữ liệu ban đầu
            var monAnList = _context.MonAns.AsQueryable();

            // 2. Lọc theo loại nếu có
            if (!string.IsNullOrEmpty(loai))
            {
                monAnList = monAnList.Where(ma => ma.Loai == loai);
            }

            // 3. Tìm kiếm theo tên nếu có
            if (!string.IsNullOrEmpty(search))
            {
                monAnList = monAnList.Where(ma => ma.Ten.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // 4. Lấy danh sách loại món ăn
            var loaiList = _context.MonAns
                                   .Select(ma => ma.Loai)
                                   .Distinct()
                                   .ToList();

            // 5. Gán vào ViewBag
            ViewBag.LoaiList = loaiList;        // Chỉ cần 1 biến
            ViewBag.CurrentLoai = loai;         // Lưu loại đang được chọn
                                                // 6. Trả về View
            return View(monAnList.ToList());
        }


        public IActionResult Details(int id)
        {
            var loaiList = _context.MonAns
                               .Select(ma => ma.Loai)
                               .Distinct()
                               .ToList();

            // 5. Gán vào ViewBag
            ViewBag.LoaiList = loaiList;
            var MonAn = _context.MonAns.FirstOrDefault(p => p.Id == id);
                return View(MonAn);
           
        }

       

    }
}
