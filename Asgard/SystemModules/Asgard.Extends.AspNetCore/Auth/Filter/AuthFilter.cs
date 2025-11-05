using Asgard.Abstract;
using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardUserInfo;
using Asgard.Extends.AspNetCore.Auth.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Asgard.Extends.AspNetCore.Auth.Filter
{
    /// <summary>
    /// 容器权限过滤器
    /// </summary>
    public class AuthFilter : IAuthorizationFilter
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly AbsLogger _logger;

        private readonly bool _needsTenantID;
        private readonly AsgardContext _context;
        private readonly string[] _roles;
        private readonly string[] _posts;
        private readonly string[] _customData;

        /// <summary>
        /// 
        /// </summary> 
        public AuthFilter(string[] roles, string[] posts, string[] customData, bool needsTenantID, AsgardContext context, AbsLoggerProvider logger)
        {
            _roles = roles;
            _logger = logger.CreateLogger<AuthFilter>();
            _context = context;
            _posts = posts;
            _customData = customData;
            _needsTenantID = needsTenantID;
        }
        /// <summary>
        /// 验证发生时触发的函数
        /// </summary>
        /// <param name="filterContext"></param> 
        public virtual void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            try
            {
                if (_context.Auth is null)
                {
                    filterContext.Result = new ForbidResult(403, "{\"msg\":\"权限模块未初始化. \"}");
                    return;
                }
                if (filterContext == null || filterContext.Filters.Any(item => item is IAllowAnonymous))
                {
                    return;
                }

                if (!filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out var rawToken)
                    || rawToken.Count != 1
                    || !rawToken[0]!.StartsWith("Bearer ")
                    )
                {
                    if (
                        !filterContext.HttpContext.Request.Query.TryGetValue("Authorization", out rawToken)
                        || rawToken.Count != 1
                        || !rawToken[0]!.StartsWith("Bearer ")
                        )
                    {
                        filterContext.Result = new ForbidResult(403, "{\"msg\":\"Token是空. \"}");
                        return;
                    }
                }
                var token = rawToken[0]!.Replace("Bearer ", "");

                if (!_context.Auth.TryGetUserInfo(token, out var userInfo, out var jti, out var tokenType) || userInfo is null || tokenType != TokenType.AccessToken)
                {
                    filterContext.Result = new ForbidResult(401, "{\"msg\":\"非法token. \"}");
                    return;
                }

                if (_needsTenantID && userInfo.CurrentTID < 0)
                {
                    filterContext.Result = new ForbidResult(401, "{\"msg\":\"请使用商户账号!\"}");
                    return;
                }
                if (_roles.Length != 0
                    && !userInfo.Roles.Contains("*")
                    && !userInfo.Roles.Intersect(_roles).Any())
                {
                    filterContext.Result = new ForbidResult(401, "{\"msg\":\"角色权限不足!\"}");
                    return;
                }
                if (_posts.Length != 0
                    && !userInfo.Posts.Contains("*")
                    && !userInfo.Posts.Intersect(_posts).Any())
                {
                    filterContext.Result = new ForbidResult(401, "{\"msg\":\"岗位权限不足!\"}");
                    return;
                }

                if (_customData.Length != 0
                    && !userInfo.CustomInfo.Keys.Intersect(_customData).Any())
                {
                    filterContext.Result = new ForbidResult(401, "{\"msg\":\"用户自定义的权限不足!\"}");
                    return;
                }

                return;
            }
            catch (Exception ex)
            {
                _logger?.Information($"用户检查失败.", exception: ex);
            }
            filterContext.Result = new ForbidResult(500, "{\"msg\":\"异常的权限请求!\"}");
        }
    }
}
