using JustLearn1.Data;
using JustLearn1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AssignmentController : Controller
{
    private readonly JustDbContext _context;

    public AssignmentController(JustDbContext context)
    {
        _context = context;
    }

    public IActionResult Create(int productId)
    {
        System.Diagnostics.Debug.WriteLine($"Received ProductID: {productId}");
        var assignment = new Assignment { ProductID = productId };
        return View(assignment);
    }
    public async Task<IActionResult> ListAssignmentsForProduct(int productId)
    {
        // ProductId'ye göre tüm assignmentları getir.
        var assignments = await _context.Assignments
                                        .Where(a => a.ProductID == productId)
                                        .ToListAsync();

        // İlgili Product bilgisini getir (Örn. ürün adını başlıkta göstermek için)
        var product = await _context.Products
                                    .FirstOrDefaultAsync(p => p.Id == productId);

        // Verileri bir ViewModel'e aktarabilir veya doğrudan view'e iletebilirsiniz.
        ViewBag.ProductName = product?.Name ?? "Unknown Product";
        return View(assignments);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Assignment assignment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyProducts", "Products");
        }

        // Debugging: check which model state is invalid
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in errors)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
        }

        return View(assignment);
    }


    public async Task<IActionResult> Edit(int id)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment == null)
        {
            return NotFound();
        }
        return View(assignment);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Assignment assignment)
    {
        if (id != assignment.AssignmentID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(assignment);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyProducts", "Product");
        }
        return View(assignment);
    }
    [HttpPost]
    public async Task<IActionResult> UploadAssignmentFile(int assignmentId, IFormFile file)
    {
        // Dosya türü ve boyutu kontrolü...

        var path = Path.Combine("uploads", file.FileName); // Güvenli ve benzersiz bir yol oluşturun!
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var assignmentFile = new AssignmentFile
        {
            FileName = file.FileName,
            FilePath = path,
            AssignmentId = assignmentId
            // Diğer özellikler...
        };

        _context.Add(assignmentFile);
        await _context.SaveChangesAsync();

        return RedirectToAction("MyAssignments"); // Veya uygun olan bir başka yönlendirme
    }
    public async Task<IActionResult> ReviewAssignments(int assignmentId)
    {
        var assignmentFiles = await _context.AssignmentFiles
                                            .Where(af => af.AssignmentId == assignmentId)
                                            .ToListAsync();
        return View(assignmentFiles);
    }


    // Diğer metodlar: Delete, Details, vb.
}
