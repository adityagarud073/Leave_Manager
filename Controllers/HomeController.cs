using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Controllers;

public class HomeController : Controller
{
  
    public IActionResult About()
    {
        return View();
    }

     public IActionResult Index()
    {
        return View();
    }

   
}
