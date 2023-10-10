using JustLearn1.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JustLearn1.Controllers
{
    public class HomeController : Controller
    {
        private IProductRepository productRepository;
        public HomeController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IActionResult Index()
        {
            return View(productRepository.GetTrendingProducts());       
        }
    }
}
