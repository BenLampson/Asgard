using Asgard.Abstract;
using Asgard.Abstract.Logger;
using Asgard.Extends.AspNetCore;
using Asgard.Extends.AspNetCore.ApiModels;
using Asgard.Extends.AspNetCore.Auth;

using Microsoft.AspNetCore.Mvc;

namespace Asgard.AspNetCore.Full.Controllers
{
    /// <summary>
    /// 演示
    /// </summary>
    [ApiController]
    [Route("Asgard/[Controller]")]
    [ApiExplorerSettings(GroupName = "Asgard.AspNetCore.Full")]
    public class HelloWorldController : APIControllerBase
    {
        public HelloWorldController(AsgardContext context, AbsLoggerProvider logger) : base(context, logger)
        {
        }

        /// <summary>
        /// demo
        /// </summary>
        [HttpGet("")]
        [Auth()]
        public virtual DataResponse<string> ImportArticleDatas()
        {
            throw new Exception("");
            //return HandleData(Guid.NewGuid().ToString());
        }

    }
}
