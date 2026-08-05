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

        return View();
    }

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

        leave.UserId = HttpContext.Session.GetInt32("UserId").Value;
        leave.Status = "Pending";

        _context.Leaves.Add(leave);
        _context.SaveChanges();

        return RedirectToAction("MyLeaves");
    }

    public IActionResult MyLeaves()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        int userId = HttpContext.Session.GetInt32("UserId").Value;

        var leaves = _context.Leaves
            .Where(x => x.UserId == userId)
            .ToList();

        return View(leaves);
    }
}