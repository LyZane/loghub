﻿using Microsoft.AspNetCore.Http;
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
        public ApiResult Get()
        {
            return ApiResult.Sucess(null, "ok");
        }

        [HttpPost]
        public ApiResult Post(IFormFile file)
        {
            try
            {
                LogPackageReceiver.Receive(file, Request.Headers["Content-MD5"], Request.Headers["ApplicationId"][0], Request.HttpContext.Connection.RemoteIpAddress.ToString());
                return ApiResult.Sucess();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail(ex);
            }
            
        }
    }
}
