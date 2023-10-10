using JustLearn1.Data;
using JustLearn1.Models.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http; // IHttpContextAccessor için gerekli
using System.Linq; // FirstOrDefault için gerekli
using System.Collections.Generic; // List için gerekli
using System;

namespace JustLearn1.Models.Services
{
    public class OrderRepository : IOrderRepository
    {
        private JustDbContext dbContext;
        private IShoppingCartRepository shoppingCartRepository;
        private IHttpContextAccessor httpContextAccessor;

        // httpContextAccessor'u constructor'a parametre olarak ekleyin.
        public OrderRepository(JustDbContext dbContext, IShoppingCartRepository shoppingCartRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.shoppingCartRepository = shoppingCartRepository;
            this.httpContextAccessor = httpContextAccessor; // Ve burada atama yapın.
        }

        public void PlaceOrder(Order order)
        {
            var shoppingCartItems = shoppingCartRepository.GetShoppingCartItems();
            order.OrderDetails = new List<OrderDetail>();

            // Giriş yapmış kullanıcının ID'sini alın
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            // Kullanıcının tamamını veritabanından alın
            var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} could not be found.");
            }

            foreach (var item in shoppingCartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Quantity = item.Qty,
                    ProductId = item.Product.Id,
                    Price = item.Product.Price,
                    User = user,  // Kullanıcıyı ayarlayın
                    Username = user.UserName
                };
                order.OrderDetails.Add(orderDetail);
            }


            order.OrderPlaced = DateTime.Now;
            order.OrderTotal = shoppingCartRepository.GetShoppingCartTotal();
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
        }
    }
}
