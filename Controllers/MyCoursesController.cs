using JustLearn1.Data;
using JustLearn1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace JustLearn1.Controllers
{
    public class MyCoursesController : Controller
    {
        private readonly JustDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyCoursesController(JustDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Assignment).ToListAsync();
            var userId = _userManager.GetUserId(User);

            var purchasedProducts = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.UserId == userId)
                .Select(od => od.Product)
                .Distinct() 
                .ToListAsync();

            return View(purchasedProducts);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = _userManager.GetUserId(User);
            bool hasPurchased = await _context.OrderDetails
                .AnyAsync(od => od.UserId == userId && od.ProductId == id);

            if (!hasPurchased)
            {
                return Forbid(); 
            }


            var product = await _context.Products
                .Include(p => p.Assignment) 
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(product);
        }

            public async Task<IActionResult> StartAssignment(int assignmentId)
            {
                var assignment = await _context.Assignments
                                               .Include(a => a.Product)  // İlgili ürün bilgilerini çekiyoruz.
                                               .FirstOrDefaultAsync(a => a.AssignmentID == assignmentId);

                if (assignment == null)
                {
                    return NotFound(); 
                }

                return View(assignment);  
            }
        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(int AssignmentId, string UserComment)
        {
            var userId = _userManager.GetUserId(User);


            var userAssignment = await _context.UserAssignments.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AssignmentId == AssignmentId);

            if (userAssignment == null)
            {

                userAssignment = new UserAssignment
                {
                    UserId = userId,
                    AssignmentId = AssignmentId,
                    IsSubmitted = true,
                    SubmissionDate = DateTime.Now,
                    UserAnswer = UserComment 
                };
                _context.UserAssignments.Add(userAssignment);
            }
            else
            {

                userAssignment.IsSubmitted = true;
                userAssignment.SubmissionDate = DateTime.Now;
                userAssignment.UserAnswer = UserComment;
            }

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }




    }
}

