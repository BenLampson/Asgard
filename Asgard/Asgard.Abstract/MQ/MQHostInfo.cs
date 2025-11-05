namespace Asgard.Abstract.MQ
{
    /// <summary>
    /// MQ的主机信息
    /// </summary>
    public class MQHostInfo
    {
        /// <summary>
        /// 主机信息
        /// </summary>
        public string Host { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 密码
        /// </summary>
        public string Passwords { get; set; } = string.Empty;

    }
}
