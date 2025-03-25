using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Asm5_BanHoanChinh1.Models;

namespace Asm5_BanHoanChinh1.Controllers
{
    public class AccountAdminController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountAdminController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var th = user;
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Manager");
                }
            }
            ViewBag.Error = "Invalid login attempt";
            return View();
        }

        public IActionResult Register()
        {
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
                VaiTro = "AdminViewProduct"
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Gán vai trò cho người dùng
                await _userManager.AddClaimAsync(user, new Claim("AdminViewProduct", email));

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "AccountAdmin");
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
