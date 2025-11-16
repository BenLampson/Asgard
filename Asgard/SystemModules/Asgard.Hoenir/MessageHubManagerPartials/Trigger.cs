using System.Runtime.CompilerServices;

namespace Asgard.Hoenir
{
    /// <summary>
    /// 消息中心管理器
    /// </summary>
    public partial class MessageHubManager
    {
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
            List<Task<MessageDataItem?>> tasks = [];
            List<MessageDataItem?> res = [];

            if (data is not null && IsHandled(data))
            {
                if (LogDetailInfo)
                {
                    Logger?.Trace($"消息重复,{data.MessageID}", eventID: GlobalSessionID);
                }
                return res;
            }
            if (LogDetailInfo)
            {
                Logger?.Trace($"尝试发起异步, MessageKey:{messageKey} data:{(data == null ? "无数据" : System.Text.Json.JsonSerializer.Serialize(data))} 发起文件:{filePath} 行号:{num}", eventID: GlobalSessionID);
            }

            if (_callBackPool.TryGetValue(messageKey, out var cbs))
            {
                data ??= new MessageDataItem();
                data.FromFile = filePath;
                data.Line = num;
                foreach (var item in cbs)
                {
                    if (data is null) continue;
                    if (Tools.Wildcard.IsMatch(item.Key, data?.To ?? "*"))
                    {
                        try
                        {
                            tasks.Add(item.Value(data));
                        }
                        catch (Exception ex)
                        {
                            Logger?.Error($"调用函数出错.{item.Key}", eventID: GlobalSessionID, exception: ex);
                        }
                    }
                }
            }

            if (wait)
            {
                var results = await Task.WhenAll(tasks);
                res.AddRange(results);
            }

            return res;
        }

    }
}
