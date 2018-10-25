/* This uses code from 2018_Semester1_Week7\Lecture6\Data */

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MagicInventoryWebsite.Models;


namespace MagicInventoryWebsite.Controllers
{
    public class HomeController : Controller
    {
        
      
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult OwnerHomePage()
        {
            ViewData["Message"] = "Your the owner.";

            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
