using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Asm5_BanHoanChinh1.Models;

namespace Asm5_BanHoanChinh1.data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
		{
		}
	
        public DbSet<MonAnModel> MonAns { get; set; }
       
        public DbSet<DonHangModel> DonHangs { get; set; }
        public DbSet<ChiTietDonHangModel> ChiTietDonHangs { get; set; }
    }
}
