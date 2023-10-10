using JustLearn1.Models;

namespace JustLearn1.ViewModel
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
