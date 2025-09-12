namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel
{
    /// <summary>
    /// MQ服务器配置
    /// </summary>
    public class MQConfig
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; } = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Server { get; set; } = "amqp://127.0.0.1:5672";
    }
}
