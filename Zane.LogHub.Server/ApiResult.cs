using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zane.LogHub.Server
{
    public class ApiResult : IActionResult
    {
        private ApiResult() { }
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public object Data { get; private set; }

        public static ApiResult Fail(string message)
        {
            return new ApiResult() { Success = false, Message = message };
        }

        public static ApiResult Sucess(object data, string message = "success")
        {
            return new ApiResult() { Success = true, Data = data, Message = message };
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await new ObjectResult(this).ExecuteResultAsync(context);

            //    var response = new HttpResponseMessage()
            //{
            //    Content = new ObjectContent(typeof(ApiResult), this, new JsonMediaTypeFormatter()),
            //    RequestMessage = null
            //};
            //return Task.FromResult(response);
        }
    }
}
