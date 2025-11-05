using System.Runtime.CompilerServices;

namespace Asgard.Hoenir
{
    /// <summary>
    /// 消息中心管理器
    /// </summary>
    public partial class MessageHubManager
    {
        /// <summary>
        /// 注册一个独立事件，该类型的事件只能存在一个实例        
        /// </summary>
        /// <remarks>该类事件如果注册多次，会以最后一次为准</remarks>
        /// <param name="messageKey">事件Key</param>
        /// <param name="cb">回调函数</param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        public bool RegistSingleCB(string messageKey, Func<MessageDataItem?, MessageDataItem?> cb, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            Logger?.Trace($"尝试注册独立事件回调, MessageKey:{messageKey} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);


            if (!_singleCallBackPool.ContainsKey(messageKey))//查看又没有这个事件注册,比如完全没有
            {
                _ = _singleCallBackPool.Remove(messageKey, out var _);//我就创建这个事件给池
            }
            return _singleCallBackPool.TryAdd(messageKey, cb);
        }

        /// <summary>
        /// 取消一个事件注册，该类型的事件只能存在一个实例        
        /// </summary>
        /// <param name="messageKey">事件Key</param>
        /// <param name="filePath">调用文件地址</param>
        /// <param name="num">调用行号</param> 
        public void UnRegistSingleCB(string messageKey, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            Logger?.Trace($"尝试注销回调, MessageKey:{messageKey} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);

            _ = _singleCallBackPool.Remove(messageKey, out var _);//我就创建这个事件给池
        }

    }
}
