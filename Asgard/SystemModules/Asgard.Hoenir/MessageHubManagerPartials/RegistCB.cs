using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Asgard.Hoenir
{
    /// <summary>
    /// 消息中心管理器
    /// </summary>
    public partial class MessageHubManager
    {
        /// <summary>
        /// 注册一个事件
        /// </summary>
        /// <param name="messageKey">事件Key</param>
        /// <param name="cb">回调函数</param>
        /// <param name="cbID">注册ID</param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        public void RegistCB(string messageKey, Func<MessageDataItem?, Task<MessageDataItem?>> cb, string cbID, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            Logger?.Trace($"尝试注册回调, MessageKey:{messageKey} cbID:{cbID} 发起文件:{filePath} 行号:{num}", eventID: GlobalSessionID);

            _ = _callBackPool.AddOrUpdate(messageKey, (key) =>
             {
                 var data = new ConcurrentDictionary<string, Func<MessageDataItem?, Task<MessageDataItem?>>>();
                 _ = data.TryAdd(cbID, cb);
                 return data;
             },
             (k, old) =>
             {
                 _ = old.AddOrUpdate(cbID, cb, (cbKey, oldInfo) =>
                 {
                     return cb;
                 });
                 return old;
             });
        }


        /// <summary>
        /// 取消一个事件注册
        /// </summary>
        /// <param name="messageKey">事件Key</param>
        /// <param name="cbID">注册ID</param>
        /// <param name="filePath">调用文件地址</param>
        /// <param name="num">调用行号</param> 
        public void UnRegistCB(string messageKey, string cbID, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            Logger?.Trace($"尝试注销回调, MessageKey:{messageKey} 发起文件:{filePath} 行号:{num}", eventID: GlobalSessionID);


            if (_callBackPool.TryGetValue(messageKey, out var cbs))//尝试用事件Key,获取一下这个事件Key所有的回调池
            {
                _ = cbs.TryRemove(cbID, out var _);//直接移除对应的回调ID
            }
        }

    }
}
