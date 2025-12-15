using Microsoft.AspNetCore.Mvc;

namespace JeerowayWiki.Images.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}