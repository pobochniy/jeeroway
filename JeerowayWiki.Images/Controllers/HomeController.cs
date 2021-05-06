using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JeerowayWiki.Images.Controllers
{
    //[Route("[controller]")]
    public class HomeController : Controller
    {
        //[HttpGet("[action]")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
