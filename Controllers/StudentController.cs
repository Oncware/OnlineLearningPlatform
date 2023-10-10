using JustLearn1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class StudentsController : Controller
{
    private readonly JustDbContext _context;

    public StudentsController(JustDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int productId)
    {
        var totalAssignments = await _context.Assignments
            .Where(a => a.ProductID == productId).CountAsync();

        var usersWhoBoughtProduct = await _context.OrderDetails
            .Where(od => od.ProductId == productId)
            .Include(od => od.User)
            .Select(od => new StudentViewModel
            {
                UserName = od.User.UserName,
                UserId = od.UserId,
                UserAssignments = _context.UserAssignments
                                    .Where(ua => ua.UserId == od.UserId && ua.Assignment.ProductID == productId)
                                    .ToList(),

                Process = totalAssignments == 0
                          ? 0
                          : (_context.UserAssignments
                              .Where(ua => ua.UserId == od.UserId
                                          && ua.Assignment.ProductID == productId && ua.IsSubmitted)
                              .Count() / (decimal)totalAssignments) * 100
            })
            .ToListAsync();

        usersWhoBoughtProduct = usersWhoBoughtProduct
            .GroupBy(u => u.UserId)
            .Select(g => g.First())
            .ToList();

        return View(usersWhoBoughtProduct);
    }



    public async Task<IActionResult> GradeAssignment(int assignmentId)
    {
        var userAssignment = await _context.UserAssignments
            .Include(ua => ua.Assignment)
            .Include(ua => ua.User)
            .FirstOrDefaultAsync(ua => ua.Id == assignmentId);

        if (userAssignment == null)
        {
            return NotFound();
        }

        return View(userAssignment);
    }


    [HttpPost]
    public async Task<IActionResult> GradeAssignment(UserAssignment updatedUserAssignment)
    {
        var originalAssignment = await _context.UserAssignments.FindAsync(updatedUserAssignment.Id);

        if (originalAssignment == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            originalAssignment.Score = updatedUserAssignment.Score;
            originalAssignment.IsSubmitted = updatedUserAssignment.IsSubmitted;
            originalAssignment.SubmissionDate = updatedUserAssignment.SubmissionDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserAssignments.Any(ua => ua.Id == updatedUserAssignment.Id))
                {
                    return NotFound(); // Handle not found
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(updatedUserAssignment); // If model is not valid, return to the view with existing data
    }

}
