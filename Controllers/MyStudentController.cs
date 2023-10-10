using JustLearn1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MyStudentsController : Controller
{
    private readonly JustDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MyStudentsController(JustDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int productId)
    {
        var userAssignments = await _context.OrderDetails
            .Where(od => od.ProductId == productId)
            .SelectMany(od => _context.UserAssignments
                .Where(ua => ua.UserId == od.UserId && ua.Assignment.ProductID == productId)
                .Include(ua => ua.User)
                .Include(ua => ua.Assignment))
            .ToListAsync();

        return View(userAssignments);
    }


    public IActionResult Grade(int id)
    {
        var userAssignment = _context.UserAssignments.Find(id);
        if (userAssignment == null)
        {

        }
        return View(userAssignment);
    }


    [HttpPost]
    public async Task<IActionResult> Grade(UserAssignment userAssignment)
    {
        if (ModelState.IsValid)
        {
            _context.Update(userAssignment);
            await _context.SaveChangesAsync();
            return RedirectToAction("ReviewAssignments", new { assignmentId = userAssignment.AssignmentId });
        }

        return View(userAssignment);
    }

}
