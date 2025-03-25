using Asm5_BanHoanChinh1.data;

namespace Asm5_BanHoanChinh1.Models
{
    public class DonHangModel
    {
        public int Id { get; set; }
        public string? KhachHangId { get; set; }
   
        public DateTime? NgayDat { get; set; }
        public string TrangThai { get; set; }
        public decimal TongTien { get; set; }
        public virtual ICollection<ChiTietDonHangModel> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangModel>();
        public virtual ApplicationUser? MaNguoiDungNavigation { get; set; }
    }
}
