using Asgard.Core.ContextModules.AspNet.ApiModels;
using Asgard.Core.ContextModules.AspNet.Auth;
using Asgard.Core.ContextModules.Logger.AbsInfos;

using Microsoft.AspNetCore.Mvc;

namespace Asgard.Core.ContextModules.AspNet
{
    /// <summary>
    /// 容器API抽象基类
    /// </summary>
    [APIServer]
    public class APIControllerBase : ControllerBase
    {
        /// <summary>
        /// 阿斯加德上下文
        /// </summary>
        protected AsgardContext Context { get; private set; }
        /// <summary>
        /// 阿斯加德上下文
        /// </summary>
        protected AbsLogger Logger { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public APIControllerBase(AsgardContext context, AbsLogger logger)
        {
            Context = context;
            Logger = logger;
        }

        /// <summary>
        /// 无效参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected DataResponse<T> InvalidParameter<T>(string? msg = null)
        {
            msg += $" [{Context.EventID}]";
            return new DataResponse<T>()
            {
                Code = ResponseCodeEnum.Error | ResponseCodeEnum.Parameter,
                Data = default,
                Msg = msg
            };
        }

        /// <summary>
        /// 系统内部错误
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected DataResponse<T> SystemError<T>(string? msg = null)
        {
            msg += $" [{Context.EventID}]";
            return new DataResponse<T>()
            {
                Code = ResponseCodeEnum.Error | ResponseCodeEnum.System,
                Data = default,
                Msg = msg
            };
        }

        /// <summary>
        /// 非法请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected DataResponse<T> IllegalRequest<T>(string? msg = null)
        {
            msg += $" [{Context.EventID}]";
            return new DataResponse<T>()
            {
                Code = ResponseCodeEnum.Error | ResponseCodeEnum.Logic,
                Data = default,
                Msg = msg
            };
        }

        /// <summary>
        /// 正常数据处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected DataResponse<T> HandleData<T>(T data, string? msg = null)
        {
            msg += $" [{Context.EventID}]";
            return new DataResponse<T>()
            {
                Code = ResponseCodeEnum.Success,
                Data = data,
                Msg = msg
            };
        }

        /// <summary>
        /// 处理带分页的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected DataResponse<T> HandleData<T>(T data, long count, string? msg = null)
        {
            msg += $" [{Context.EventID}]";
            return new DataResponse<T>()
            {
                Code = ResponseCodeEnum.Success,
                Data = data,
                DataCount = (int)count,
                Msg = msg,
            };
        }
        /// <summary>
        /// 当前的Token
        /// </summary>
        private string? _currentToken;


        /// <summary>
        /// 当前的Token
        /// </summary>
        public string? CurrentToken
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_currentToken))
                {
                    return _currentToken;
                }
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var rawToken)
                    || rawToken.Count != 1
                    || !rawToken[0]!.StartsWith("Bearer ")
                )
                {
                    if (
                        !HttpContext.Request.Query.TryGetValue("Authorization", out rawToken)
                        || rawToken.Count != 1
                        || !rawToken[0]!.StartsWith("Bearer ")
                        )
                    {
                        return "";
                    }
                }
                _currentToken = rawToken[0]!.Replace("Bearer ", "");
                return _currentToken;

            }
        }

        /// <summary>
        /// 获取当前登录的用户信息,如果为空,则说明当前调用的接口没有权限认证
        /// </summary>
        /// <returns></returns> 
        protected UserInfo? CurrentUser
        {
            get
            {
                if (Context.Auth is null
                    || string.IsNullOrWhiteSpace(CurrentToken)
                    || !Context.Auth.TryGetUserInfo(CurrentToken, out var userInfo, out var _, out var _))
                {
                    return null;
                }
                return userInfo;
            }
        }

        /// <summary>
        /// 获取当前商户的数据库对象
        /// </summary>
        protected IFreeSql? CurrentTenantSystemDB()
        {
            var userinfo = CurrentUser;
            if (userinfo is null)
            {
                return null;
            }
            var tdInfo = userinfo.TDBInfo.FirstOrDefault();
            if (tdInfo is null)
            {
                return null;
            }
            return Context.DB.GetMyDB($"{userinfo.CurrentTID}_Default", tdInfo.ConnStr, tdInfo.Type, tdInfo.ReadDB);
        }

    }
}
