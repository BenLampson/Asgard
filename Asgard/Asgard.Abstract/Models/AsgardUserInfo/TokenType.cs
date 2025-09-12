namespace Asgard.Abstract.Models.AsgardUserInfo
{
    /// <summary>
    /// token类型
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// 非法Token
        /// </summary>
        InvalidToken = 0,
        /// <summary>
        /// 刷新Token
        /// </summary>
        RefreshToken = 1,
        /// <summary>
        /// 访问Token
        /// </summary>
        AccessToken = 2
    }
}
