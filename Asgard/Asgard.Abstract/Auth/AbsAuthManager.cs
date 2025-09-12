using Asgard.Abstract.Models.AsgardUserInfo;

namespace Asgard.Abstract.Auth
{
    /// <summary>
    /// 认证管理器抽象基类
    /// 提供JWT Token的创建、解析和刷新功能的基础实现接口
    /// </summary>
    public abstract class AbsAuthManager
    {
        /// <summary>
        /// jwt密钥
        /// </summary>
        protected readonly string _jwtKey;
        /// <summary>
        /// 听众,也就是使用者
        /// </summary>
        protected readonly string _audience;
        /// <summary>
        /// 颁发者
        /// </summary>
        protected readonly string _issuer;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="jwtKey">密钥RSA512</param>
        /// <param name="issuer">颁发人</param>
        /// <param name="audience">听众</param> 
        public AbsAuthManager(string jwtKey, string issuer, string audience)
        {
            _audience = audience;
            _issuer = issuer;
            _jwtKey = jwtKey;
        }

        /// <summary>
        /// 解密获取某一个用户信息
        /// </summary>
        public abstract bool TryGetUserInfo(string rawToken, out UserInfo? userInfo, out string? jti, out TokenType type);

        /// <summary>
        /// 尝试创建一个Token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="token"></param>
        /// <param name="delayWork"></param>
        /// <param name="expire"></param> 
        /// <returns></returns>
        public abstract bool TryCreateToken(UserInfo userInfo, out string? token, DateTime? delayWork, DateTime? expire);



        /// <summary>
        /// 创建一个超长期的刷新Token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="token"></param>
        /// <param name="jti"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public abstract bool TryCreateRefreshToken(UserInfo userInfo, out string token, out string jti, DateTime? expires = null);


    }
}
