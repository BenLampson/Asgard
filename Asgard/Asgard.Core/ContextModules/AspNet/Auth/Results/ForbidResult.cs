
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace Asgard.Core.ContextModules.AspNet.Auth.Results
{
    /// <summary>
    /// 异常返回
    /// </summary>
    public class ForbidResult : IActionResult
    {
        private readonly string _jsonData;
        private readonly int _statusCode;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="statusCode">http状态码</param>
        /// <param name="jsonData">json数据</param>
        public ForbidResult(int statusCode, string jsonData)
        {
            _statusCode = statusCode;
            _jsonData = jsonData;
        }
        /// <summary>
        /// 响应
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ExecuteResultAsync(ActionContext context) => Task.Run(async () =>
        {
            context.HttpContext.Response.StatusCode = _statusCode;
            if (!string.IsNullOrWhiteSpace(_jsonData))
            {
                context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
                await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(_jsonData));
            }
        });
    }
}
