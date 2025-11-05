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
        /// <param name="messageKey"></param>
        /// <param name="data"></param>
        /// <param name="ignoreEx"></param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        /// <returns>返回的值是每个监听的事件给你的回复,key是订阅ID,value是这个订阅ID的返回值</returns>
        public ICollection<MessageDataItem?> Trigger(string messageKey, MessageDataItem? data = null, bool ignoreEx = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            List<MessageDataItem?> res = new();
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
                Logger?.Trace($"尝试发起同步消息, MessageKey:{messageKey} data:{(data == null ? "无数据" : System.Text.Json.JsonSerializer.Serialize(data))} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);
            }
            if (_callBackPool.TryGetValue(messageKey, out var cbs))
            {
                foreach (var item in cbs)
                {
                    if (Tools.Wildcard.IsMatch(item.Key, data?.To ?? ""))
                    {

                    }
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
                        var loopRes = item.Value.Invoke(data);
                        res.Add(loopRes);
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error($"调用函数出错.{item.Key}", eventID: IDESessionID, exception: ex);
                        if (!ignoreEx)
                        {
                            throw new Exception(ex.Message, ex);
                        }

                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 异步触发 如果等待结果请await
        /// </summary>
        /// <param name="messageKey"></param>
        /// <param name="data"></param>
        /// <param name="wait">是否等待</param>
        /// <param name="filePath">调用文件地址</param>
        /// <param name="num">调用行号</param> 
        /// <returns>返回的值是每个监听的事件给你的回复,key是订阅ID,value是这个订阅ID的返回值</returns>
        public async Task<ICollection<MessageDataItem?>> TriggerAsync(string messageKey, MessageDataItem? data = null, bool wait = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            var tasks = new List<Task>();
            List<MessageDataItem?> res = new();
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

            if (_callBackPool.TryGetValue(messageKey, out var cbs))//尝试用事件Key,获取一下这个事件Key所有的回调池
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
                foreach (var item in cbs)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            var loopRes = item.Value.Invoke(data);
                            lock (res)
                            {
                                res.Add(loopRes);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger?.Error($"调用函数出错.{item.Key}", eventID: IDESessionID, exception: ex);
                        }
                    }));
                }

            }
            if (wait)
            {
                await Task.WhenAll(tasks);
            }

            return res;
        }

    }
}
