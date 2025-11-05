using Asgard.Abstract.Logger;

namespace Asgard.Abstract.MQ
{
    /// <summary>
    /// MQ管理器
    /// </summary>
    public abstract class AbsMQManager
    {
        /// <summary>
        /// 日志
        /// </summary>
        public AbsLogger Logger { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public AbsMQManager(AbsLogger logger)
        {
            Logger = logger;
        }



        /// <summary>
        /// 給本地的MQ服务发送一条消息
        /// </summary>
        /// <param name="options">消息的参数</param>
        /// <param name="message">消息内容</param>
        /// <param name="hostInfo">主机信息,为空则使用容器配置</param>
        /// <returns></returns>
        public abstract bool SendMessageTo(SendMQItemOptions options, string message, MQHostInfo? hostInfo = null);

        /// <summary>
        /// 关闭一个通道
        /// </summary>
        /// <param name="channelToken">通道key</param>
        public abstract void CloseConnection(string channelToken);

        /// <summary>
        /// 給本地的MQ服务发送一条消息
        /// </summary>
        /// <param name="options">消息的参数</param>
        /// <param name="message">消息内容</param>
        /// <param name="hostInfo">主机信息,为空则使用容器配置</param>
        /// <returns></returns>
        public abstract Task<bool> SendMessageToAsync(SendMQItemOptions options, string message, MQHostInfo? hostInfo = null);

        /// <summary>
        /// 关闭一个通道
        /// </summary>
        /// <param name="channelToken">通道key</param>
        public abstract Task CloseConnectionAsync(string channelToken);

        /// <summary>
        /// 从本地的MQ中获取消息
        /// </summary>
        /// <param name="virtualHost">虚拟主机名称</param>
        /// <param name="queueName">队列名</param>
        /// <param name="callBackMethod">回调函数</param>
        /// <param name="hostInfo">主机信息设置,不设置则使用容器配置</param>
        /// <returns></returns>
        public abstract string? GotMessageFrom(string virtualHost, string queueName, Func<string, bool> callBackMethod, MQHostInfo? hostInfo = null);

        /// <summary>
        /// 从本地的MQ中获取消息
        /// </summary>
        /// <param name="virtualHost">虚拟主机名称</param>
        /// <param name="queueName">队列名</param>
        /// <param name="callBackMethod">回调函数</param>
        /// <param name="hostInfo">主机信息设置,不设置则使用容器配置</param>       
        public abstract Task<string?> GotMessageFromAsync(string virtualHost, string queueName, Func<string, Task<bool>> callBackMethod, MQHostInfo? hostInfo = null);


    }
}
