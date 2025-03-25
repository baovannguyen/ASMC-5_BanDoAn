using Asm5_BanHoanChinh1.data;
using Asm5_BanHoanChinh1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asm5_BanHoanChinh1.Controllers
{
	[Authorize(Policy = "ViewProductPolicy")]
	public class ManagerController : Controller
	{

		private readonly ApplicationDbContext _context;


        public IActionResult Index()
        {
            return View();
        }

        public ManagerController(ApplicationDbContext context)
		{
			_context = context;
		}

       
        public IActionResult ManagerSP()
		{
			var MonAn = _context.MonAns.ToList();
			return View(MonAn);
		}

        
       

        public IActionResult Create()
		{
			return View();
		}

        // tạo sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MonAnModel MonAn)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "Lỗi: Không thể xác định người dùng hiện tại.");
                    return View(MonAn);
                }

                MonAn.CreatedBy = userId; 

                _context.MonAns.Add(MonAn);
                _context.SaveChanges();

                return RedirectToAction("ManagerSP", "Manager");
            }

            return View(MonAn);
        }




        // chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            var MonAn = _context.MonAns.FirstOrDefault(p => p.Id == id);
            if (MonAn == null)
            {
                return NotFound();
            }

            // Nếu người dùng có quyền Admin, cho phép truy cập
            if (User.HasClaim(c => c.Type == "AdminViewProduct"))
            {
                return View(MonAn);
            }

			// Nếu là nhân viên kinh doanh, chỉ cho phép nếu họ là người tạo ra sản phẩm
			var userName = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

			if (User.HasClaim(c => c.Type == "SalesViewProduct") && MonAn.CreatedBy == userName)
            {
                return View(MonAn);
            }

          
            return Forbid();
        }
     
        // Hiển thị form chỉnh sửa sản phẩm
        public IActionResult Edit(int id)
        {
            var MonAn = _context.MonAns.FirstOrDefault(p => p.Id == id);
            if (MonAn == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền người dùng
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Chỉ cho phép chỉnh sửa nếu người dùng là người tạo ra sản phẩm hoặc là Admin
            if (MonAn.CreatedBy != userName && !User.HasClaim(c => c.Type == "Admin"))
            {
                return Forbid(); // Nếu không phải người tạo hoặc Admin, không cho chỉnh sửa
            }

            return View(MonAn);
        }

        // Xử lý cập nhật sản phẩm (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MonAnModel MonAn, IFormFile fileHinhAnh)
        {
            if (id != MonAn.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMonAn = _context.MonAns.FirstOrDefault(p => p.Id == id);
                    if (existingMonAn == null)
                    {
                        return NotFound();
                    }

                    var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (existingMonAn.CreatedBy != userName && !User.HasClaim(c => c.Type == "Admin"))
                    {
                        return Forbid();
                    }

                    // Cập nhật thông tin sản phẩm
                    existingMonAn.Ten = MonAn.Ten;
                    existingMonAn.MoTa = MonAn.MoTa;
                    existingMonAn.Gia = MonAn.Gia;
                    existingMonAn.Loai = MonAn.Loai;

                    // **Bước 2: Kiểm tra nếu có file mới thì lưu và cập nhật đường dẫn**
                    if (fileHinhAnh != null && fileHinhAnh.Length > 0)
                    {
                        // Lưu ảnh vào thư mục wwwroot/img/
                        var fileName = Path.GetFileName(fileHinhAnh.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            fileHinhAnh.CopyTo(stream);
                        }

                        // Cập nhật đường dẫn ảnh vào database
                        existingMonAn.HinhAnh = fileName;
                    }

                    _context.MonAns.Update(existingMonAn);
                    _context.SaveChanges();

                    return RedirectToAction("Edit");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật sản phẩm: {ex.Message}");
                    return View(MonAn);
                }
            }

            return View(MonAn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var MonAn = _context.MonAns.FirstOrDefault(p => p.Id == id);
            if (MonAn == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền người dùng
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (MonAn.CreatedBy != userName && !User.HasClaim(c => c.Type == "Admin"))
            {
                return Forbid();
            }

            _context.MonAns.Remove(MonAn);
            _context.SaveChanges();

            return RedirectToAction("ManagerSP");
        }



    }
}
