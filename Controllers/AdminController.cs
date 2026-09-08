
using LeaveManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    private bool IsAdminLoggedIn()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");

        return userId != null && role == "Admin";
    }

    public IActionResult Index()
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        var leaveList = _context.Leaves
            .Include(x => x.User)
            .ToList();

        return View(leaveList);
    }

    public IActionResult Approve(int id)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        var leave = _context.Leaves.Find(id);

        if (leave != null)
        {
            leave.Status = "Approved";
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    public IActionResult Reject(int id)
    {
        if (!IsAdminLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        var leave = _context.Leaves.Find(id);

        if (leave != null)
        {
            leave.Status = "Rejected";
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}