using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Zane.LogHub.Server
{
    [BasicAuthorizationFilter]
    [Route("api/[controller]")]
    public class LogController : Controller
    {
        public string Get()
        {
            return "ok";
        }

        [HttpPost]
        public ActionResult Post(IFormFile file)
        {
            LogPackageReceiver.Receive(file, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            return Ok("ok");
        }
    }
}
