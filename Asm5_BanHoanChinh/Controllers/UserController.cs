using Asm5_BanHoanChinh1.data;
using Asm5_BanHoanChinh1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Asm5_BanHoanChinh1.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Login()
        {
            // 4. Lấy danh sách loại món ăn
            var loaiList = _context.MonAns
                                   .Select(ma => ma.Loai)
                                   .Distinct()
                                   .ToList();

            // 5. Gán vào ViewBag
            ViewBag.LoaiList = loaiList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
          var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "MonAn");
                }
            }
            ViewBag.Error = "Invalid login attempt";
            return View();
        }

        public IActionResult Register()
        {
            // 4. Lấy danh sách loại món ăn
            var loaiList = _context.MonAns
                                   .Select(ma => ma.Loai)
                                   .Distinct()
                                   .ToList();

            // 5. Gán vào ViewBag
            ViewBag.LoaiList = loaiList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string hoTen, string diaChi, string sdt, string ngaySinh)
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
                VaiTro = "UserViewProduct"
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Gán vai trò cho người dùng
                await _userManager.AddClaimAsync(user, new Claim("UserViewProduct", email));

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "User");
            }

            ViewBag.Error = "Registration failed. Please try again.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

