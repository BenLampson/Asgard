namespace Asgard.Abstract.MQ
{
    /// <summary>
    /// MQ消息的设置
    /// </summary>
    public class SendMQItemOptions
    {
        /// <summary>
        /// 超时时间毫秒,0不超时
        /// </summary>
        public int Expiration { get; set; }
        /// <summary>
        /// 寄送模式,1不持久化,2持久化
        /// </summary>
        public byte DeliveryMode { get; set; }
        /// <summary>
        /// 交换机名称默认:amq.direct
        /// </summary>
        public string ExchangeName { get; set; } = "amq.direct";
        /// <summary>
        /// 路由Key,默认空字符串
        /// </summary>
        public string RoutingKey { get; set; } = string.Empty;
        /// <summary>
        /// 虚拟主机的名称
        /// </summary>
        public string VirtualHostName { get; set; } = string.Empty;
        /// <summary>
        /// 是否强制
        /// </summary>
        public bool Mandatory { get; set; }
    }
}
