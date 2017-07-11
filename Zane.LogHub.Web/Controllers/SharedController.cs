using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zane.LogHub.Web.Controllers
{
    public class SharedController : Controller
    {
        public IActionResult LeftSidebar()
        {
            return View();
        }
    }
}