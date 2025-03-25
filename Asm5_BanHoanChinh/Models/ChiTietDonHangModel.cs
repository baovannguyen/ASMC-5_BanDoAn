namespace Asm5_BanHoanChinh1.Models
{
    public class ChiTietDonHangModel
    {
        public int Id { get; set; }
        public int DonHangId { get; set; }
        public DonHangModel? DonHang { get; set; }
        public int MonAnId { get; set; }
        public MonAnModel? MonAn { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
    }
}
