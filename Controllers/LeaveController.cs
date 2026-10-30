

using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class LeaveController : Controller
{
    private readonly AppDbContext _context;

    public LeaveController(AppDbContext context)
    {
        _context = context;
    }

    private bool IsLoggedIn()
    {
        return HttpContext.Session.GetInt32("UserId") != null;
    }

    public IActionResult Apply()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        int userId = HttpContext.Session.GetInt32("UserId").Value;

        var user = _context.Users
            .FirstOrDefault(x => x.Id == userId);

        if (user != null)
        {
            int usedLeaves = _context.Leaves
                .Where(x => x.UserId == userId &&
                            x.Status == "Approved")
                .ToList()
                .Sum(x => (x.ToDate - x.FromDate).Days + 1);

            ViewBag.TotalLeaves = user.TotalLeaves;
            ViewBag.UsedLeaves = usedLeaves;
            ViewBag.RemainingLeaves =
                user.TotalLeaves - usedLeaves;
        }

        return View();
    }

    // POST: Apply Leave
    [HttpPost]
    public IActionResult Apply(Leave leave)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        if (leave.FromDate == default || leave.ToDate == default)
        {
            TempData["Error"] = "Please select From Date and To Date.";
            return View(leave);
        }

        if (string.IsNullOrWhiteSpace(leave.Reason))
        {
            TempData["Error"] = "Reason is required.";
            return View(leave);
        }

        if (leave.FromDate > leave.ToDate)
        {
            TempData["Error"] = "From Date cannot be greater than To Date.";
            return View(leave);
        }

        int userId = HttpContext.Session.GetInt32("UserId").Value;

        var user = _context.Users
            .FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            TempData["Error"] = "User not found.";

            return View(leave);
        }

        // Calculate Approved Leaves
        int usedLeaves = _context.Leaves
            .Where(x => x.UserId == userId &&
                        x.Status == "Approved")
            .ToList()
            .Sum(x => (x.ToDate - x.FromDate).Days + 1);

        int remainingLeaves = user.TotalLeaves - usedLeaves;

        int requestedDays = (leave.ToDate - leave.FromDate).Days + 1;

        if (remainingLeaves <= 0)
        {
            TempData["Error"] = "No Leave Balance Remaining.";

            return View(leave);
        }

        if (requestedDays > remainingLeaves)
        {
            TempData["Error"] = $"Only {remainingLeaves} leave(s) remaining.";

            return View(leave);
        }

        leave.UserId = userId;
        leave.Status = "Pending";

        _context.Leaves.Add(leave);
        _context.SaveChanges();

        TempData["Success"] = "Leave Applied Successfully.";

        return RedirectToAction("MyLeaves");
    }

    // My Leaves
    public IActionResult MyLeaves()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        int userId =
            HttpContext.Session.GetInt32("UserId").Value;

        var user = _context.Users.FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        int usedLeaves = _context.Leaves
            .Where(x => x.UserId == userId &&
                        x.Status == "Approved")
            .ToList()
            .Sum(x => (x.ToDate - x.FromDate).Days + 1);

        int remainingLeaves = user.TotalLeaves - usedLeaves;

        ViewBag.TotalLeaves = user.TotalLeaves;
        ViewBag.UsedLeaves = usedLeaves;
        ViewBag.RemainingLeaves = remainingLeaves;

        var leaves = _context.Leaves
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Id)
            .ToList();

        return View(leaves);
    }
}