using Asm5_BanHoanChinh1.Models;

namespace Asm5_BanHoanChinh1.Models
{
    public class Cart
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal Total
        {
            get { return Quantity * Price; }
        }
        public string HinhAnh { get; set; }

        public Cart() { }
        public Cart(MonAnModel product)
        {
            ProductId = product.Id;
            ProductName = product.Ten;
            Price = product.Gia;
            Quantity = 1;
            HinhAnh = product.HinhAnh;
        }
     
    }
}
