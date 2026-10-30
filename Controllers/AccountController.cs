
using LeaveManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // public IActionResult HashAllPasswords()
    // {
    //     var users = _context.Users.ToList();

    //     foreach (var user in users)
    //     {
    //         user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    //     }

    //     _context.SaveChanges();

    //     return Content("Passwords Hashed Successfully");
    // }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == email);

        if (user == null)
        {
            ViewBag.Message = "Invalid Login";
            return View();
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            ViewBag.Message = "Invalid Login";
            return View();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Role", user.Role);
        HttpContext.Session.SetString("UserName", user.Name);

        if (user.Role == "Admin")
        {
            return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Apply", "Leave");
    }

    // [HttpPost]
    // public IActionResult Login(string email, string password)
    // {
    //     var user = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);

    //     if (user == null)
    //     {
    //         ViewBag.Message = "Invalid Login";
    //         return View();
    //     }

    //     HttpContext.Session.SetInt32("UserId", user.Id);
    //     HttpContext.Session.SetString("Role", user.Role);
    //     HttpContext.Session.SetString("UserName", user.Name);

    //     // var sessionID = HttpContext.Session.Id;

    //     // Console.WriteLine("=============================session memory=================");
    //     // Console.WriteLine("Session ID  " + sessionID);
    //     // Console.WriteLine(HttpContext.Session.GetInt32("UserId"));
    //     // Console.WriteLine(HttpContext.Session.GetString("Role"));
    //     // Console.WriteLine(HttpContext.Session.GetString("UserName"));
    //     // Console.WriteLine("UserEmail: " + user.Email);
    //     // Console.WriteLine("============================================================");



    //     if (user.Role == "Admin")
    //     {
    //         return RedirectToAction("Index", "Admin");
    //     }

    //     return RedirectToAction("Apply", "Leave");
    // }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        return RedirectToAction("Login", "Account");
    }
}