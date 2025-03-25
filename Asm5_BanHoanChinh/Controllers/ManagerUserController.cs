using Asm5_BanHoanChinh1.data;
using Asm5_BanHoanChinh1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Asm5_BanHoanChinh1.Controllers
{
    [Authorize(Policy = "ManagerPolicy")]
    public class ManagerUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public ManagerUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
			_signInManager = signInManager;
		}

        public IActionResult Index()
        {
            var users = _userManager.Users
                .Where(u => u.VaiTro == "AdminViewProduct" || u.VaiTro == "SalesViewProduct")
                .ToList();
            return View(users);
        }



        public IActionResult Index1()
        {
            var users = _userManager.Users.Where(u => u.VaiTro == "UserViewProduct").ToList();
            return View(users);
        }

        public IActionResult Details(string id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }


		[HttpPost]
		public async Task<IActionResult> Create(string email, string password, string hoTen, string diaChi, string sdt, string ngaySinh,string vaitro)
		{


			var user = new ApplicationUser
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true,
				HoTen = hoTen,
				DiaChi = diaChi,
				Sdt = sdt,
				NgaySinh = ngaySinh,
				VaiTro = vaitro
			};

			var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				// Gán vai trò cho người dùng
				await _userManager.AddClaimAsync(user, new Claim(vaitro, email));

				await _signInManager.SignInAsync(user, isPersistent: false);
				return RedirectToAction("Index", "ManagerUser");
			}

			ViewBag.Error = "Registration failed. Please try again.";
			return View();
		}

		public IActionResult Edit(string id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                return NotFound();
            }
           

                existingUser.HoTen = user.HoTen;
            existingUser.DiaChi = user.DiaChi;
            existingUser.Sdt = user.Sdt;
            existingUser.NgaySinh = user.NgaySinh;
            existingUser.VaiTro = user.VaiTro;
            // Lấy danh sách claims hiện tại của user
            var claims = await _userManager.GetClaimsAsync(existingUser);

            // Tìm claim cũ theo ClaimType cần đổi
            var oldClaim = claims.FirstOrDefault(c => c.Value == existingUser.Email); // Thay bằng ClaimType cũ
                                                                                     // Xóa claim cũ (nếu có)
            if (oldClaim != null)
            {
                await _userManager.RemoveClaimAsync(existingUser, oldClaim);
            }

            var newClaim = new Claim( user.VaiTro, existingUser.Email); 
            await _userManager.AddClaimAsync(existingUser, newClaim);
            var result = await _userManager.UpdateAsync(existingUser);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Lỗi khi xóa người dùng");
                return View("Index", _userManager.Users.ToList());
            }
            return RedirectToAction("Index");
        }
    }
}
