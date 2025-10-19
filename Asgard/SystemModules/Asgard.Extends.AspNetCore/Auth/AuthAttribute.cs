using Asgard.Extends.AspNetCore.Auth.Filter;

using Microsoft.AspNetCore.Mvc;

namespace Asgard.Extends.AspNetCore.Auth
{
    /// <summary>
    /// 权限过滤特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// 权限过滤特性
        /// </summary>
        /// <param name="roles">允许的角色</param>
        /// <param name="needsTenantID">必须拥有商户ID</param> 
        /// <param name="posts">允许的岗位Key</param>
        /// <param name="customData">用户自定义的限定</param>
        public AuthAttribute(bool needsTenantID = false, string[]? roles = null, string[]? posts = null, string[]? customData = null) : base(typeof(AuthFilter))
        {
            Arguments = new object[] { roles ?? Array.Empty<string>(), posts ?? Array.Empty<string>(), customData ?? Array.Empty<string>(), needsTenantID };
        }
    }
}
