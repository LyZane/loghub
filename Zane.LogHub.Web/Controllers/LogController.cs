using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zane.LogHub.Web.Controllers
{
    public class LogController : Controller
    {
        public IActionResult List(string appId)
        {
            return View();
        }
    }
}