using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustLearn1.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public int CreditNumber { get; set; }
        public int ExpirationDate { get; set; }
        public int CVC { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
