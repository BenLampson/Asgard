using System.Runtime.CompilerServices;

namespace Asgard.Hoenir
{
    /// <summary>
    /// 消息中心管理器
    /// </summary>
    public partial class MessageHubManager
    {
        /// <summary>
        /// 同步触发 可能会阻塞当前线程
        /// </summary>
        /// <param name="messageKey">消息key值</param>
        /// <param name="data">数据</param>
        /// <param name="ignoreEx">是否忽略异常</param>
        /// <param name="filePath">调用文件地址</param>
        /// <param name="num">调用行号</param> 
        /// <returns>返回的值是每个监听的事件给你的回复,key是订阅ID,value是这个订阅ID的返回值</returns>
        public MessageDataItem? TriggerSingle(string messageKey, MessageDataItem? data = null, bool ignoreEx = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            if (data is not null && IsHandled(data))
            {
                if (LogDetailInfo)
                {
                    Logger?.Trace($"消息重复,{data.MessageID}", eventID: IDESessionID);
                }
                return default;
            }
            if (LogDetailInfo)
            {
                Logger?.Trace($"尝试发起同步消息, MessageKey:{messageKey} data:{(data == null ? "无数据" : System.Text.Json.JsonSerializer.Serialize(data))} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);
            }
            if (_singleCallBackPool.TryGetValue(messageKey, out var item))
            {
                try
                {

                    if (data is null)
                    {
                        data = new MessageDataItem()
                        {
                            FromFile = filePath,
                            Line = num
                        };
                    }
                    else
                    {
                        data.FromFile = filePath;
                        data.Line = num;
                    }
                    return item.Invoke(data);
                }
                catch (Exception ex)
                {
                    Logger?.Error($"调用单实例函数出错:{messageKey}", eventID: IDESessionID, exception: ex);
                    if (!ignoreEx)
                    {
                        throw new Exception(ex.Message, ex);
                    }

                }
            }
            return default;
        }

        /// <summary>
        /// 异步触发 如果等待结果请await
        /// </summary>
        /// <param name="messageKey">消息key</param>
        /// <param name="data">数据</param>
        /// <param name="wait">是否等待</param>
        /// <param name="filePath">调用文件地址</param>
        /// <param name="num">调用行号</param> 
        /// <returns>返回的值是每个监听的事件给你的回复,key是订阅ID,value是这个订阅ID的返回值</returns>
        public async Task<MessageDataItem?> TriggerSingleAsync(string messageKey, MessageDataItem? data = null, bool wait = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            MessageDataItem? res = default;
            if (data is not null && IsHandled(data))
            {
                if (LogDetailInfo)
                {
                    Logger?.Trace($"消息重复,{data.MessageID}", eventID: IDESessionID);
                }
                return res;
            }
            if (LogDetailInfo)
            {
                Logger?.Trace($"尝试发起异步, MessageKey:{messageKey} data:{(data == null ? "无数据" : System.Text.Json.JsonSerializer.Serialize(data))} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);
            }

            if (_singleCallBackPool.TryGetValue(messageKey, out var item))//尝试用事件Key,获取一下这个事件Key所有的回调池
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        if (data is null)
                        {
                            data = new MessageDataItem()
                            {
                                FromFile = filePath,
                                Line = num
                            };
                        }
                        else
                        {
                            data.FromFile = filePath;
                            data.Line = num;
                        }
                        return item.Invoke(data);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error($"调用单实例函数出错.{messageKey}", eventID: IDESessionID, exception: ex);
                    }
                    return default;
                });
                if (wait)
                {
                    _ = await task;
                }

            }

            return res;
        }

    }
}
