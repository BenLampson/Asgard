using System.Runtime.CompilerServices;

using Asgard.Abstract.Logger;


namespace Asgard.Hoenir
{
    /// <summary>
    /// Message hub manager for event bus and A2A communication
    /// </summary>
    public class MessageHubManager
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static readonly MessageHubManager Instance = new();
        
        /// <summary>
        /// IDE session ID
        /// </summary>
        public static string IDESessionID { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Enable detailed logging
        /// </summary>
        public static bool LogDetailInfo { get; set; }

        /// <summary>
        /// Processed message queue for deduplication
        /// </summary>
        private readonly Queue<string> _handledMessages = new();

        /// <summary>
        /// Logger component
        /// </summary>
        private AbsLoggerProvider? _logger;
        
        /// <summary>
        /// Message hub logger provider
        /// </summary>
        internal AbsLogger? Logger { get => _logger?.CreateLogger<MessageHubManager>(); }
        
        /// <summary>
        /// Cache size for message deduplication
        /// </summary>
        public int CacheMessageKeyCount { get; set; } = 100_000;
        
        /// <summary>
        /// Number of items to clear when cache is full
        /// </summary>
        public int ClearMessageKeyCount { get; set; } = 1_000;

        /// <summary>
        /// Agent registry mapping agent IDs to callback IDs
        /// </summary>
        private readonly Dictionary<string, string> _agentRegistry = new();

        /// <summary>
        /// Registers an agent for A2A communication
        /// </summary>
        /// <param name="agentId">Unique agent identifier</param>
        /// <param name="callbackId">Callback identifier for message routing</param>
        public void RegisterAgent(string agentId, string callbackId)
        {
            _agentRegistry[agentId] = callbackId;
        }

        /// <summary>
        /// Enhanced trigger with A2A routing support
        /// </summary>
        public new ICollection<MessageDataItem?> Trigger(string messageKey, MessageDataItem? data = null, bool ignoreEx = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            // A2A unicast routing
            if (data?.RoutingMode == MessageRoutingModeEnum.Unicast && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                if (_agentRegistry.TryGetValue(data.TargetAgentId, out var targetCallbackId))
                {
                    var agentMessageKey = $"agent:{data.TargetAgentId}:{messageKey}";
                    return base.Trigger(agentMessageKey, data, ignoreEx, filePath, num);
                }
                return new List<MessageDataItem?>(); // No agent found
            }

            // A2A group multicast
            if (data?.RoutingMode == MessageRoutingModeEnum.Group && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                var groupMessageKey = $"group:{data.TargetAgentId}:{messageKey}";
                return base.Trigger(groupMessageKey, data, ignoreEx, filePath, num);
            }

            // Original broadcast behavior (unchanged)
            return base.Trigger(messageKey, data, ignoreEx, filePath, num);
        }

        /// <summary>
        /// Enhanced async trigger with A2A routing support
        /// </summary>
        public new async Task<ICollection<MessageDataItem?>> TriggerAsync(string messageKey, MessageDataItem? data = null, bool wait = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            // A2A unicast routing
            if (data?.RoutingMode == MessageRoutingModeEnum.Unicast && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                if (_agentRegistry.TryGetValue(data.TargetAgentId, out var targetCallbackId))
                {
                    var agentMessageKey = $"agent:{data.TargetAgentId}:{messageKey}";
                    return await base.TriggerAsync(agentMessageKey, data, wait, filePath, num);
                }
                return new List<MessageDataItem?>(); // No agent found
            }

            // A2A group multicast
            if (data?.RoutingMode == MessageRoutingModeEnum.Group && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                var groupMessageKey = $"group:{data.TargetAgentId}:{messageKey}";
                return await base.TriggerAsync(groupMessageKey, data, wait, filePath, num);
            }

            // Original broadcast behavior (unchanged)
            return await base.TriggerAsync(messageKey, data, wait, filePath, num);
        }

        /// <summary>
        /// Enhanced single trigger with A2A routing support
        /// </summary>
        public new MessageDataItem? TriggerSingle(string messageKey, MessageDataItem? data = null, bool ignoreEx = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            // A2A unicast routing
            if (data?.RoutingMode == MessageRoutingModeEnum.Unicast && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                if (_agentRegistry.TryGetValue(data.TargetAgentId, out var targetCallbackId))
                {
                    var agentMessageKey = $"agent:{data.TargetAgentId}:{messageKey}";
                    return base.TriggerSingle(agentMessageKey, data, ignoreEx, filePath, num);
                }
                return default; // No agent found
            }

            // A2A group multicast (treated as broadcast for single events)
            if (data?.RoutingMode == MessageRoutingModeEnum.Group && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                var groupMessageKey = $"group:{data.TargetAgentId}:{messageKey}";
                return base.TriggerSingle(groupMessageKey, data, ignoreEx, filePath, num);
            }

            // Original broadcast behavior (unchanged)
            return base.TriggerSingle(messageKey, data, ignoreEx, filePath, num);
        }

        /// <summary>
        /// Enhanced async single trigger with A2A routing support
        /// </summary>
        public new async Task<MessageDataItem?> TriggerSingleAsync(string messageKey, MessageDataItem? data = null, bool wait = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            // A2A unicast routing
            if (data?.RoutingMode == MessageRoutingModeEnum.Unicast && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                if (_agentRegistry.TryGetValue(data.TargetAgentId, out var targetCallbackId))
                {
                    var agentMessageKey = $"agent:{data.TargetAgentId}:{messageKey}";
                    return await base.TriggerSingleAsync(agentMessageKey, data, wait, filePath, num);
                }
                return default; // No agent found
            }

            // A2A group multicast (treated as broadcast for single events)
            if (data?.RoutingMode == MessageRoutingModeEnum.Group && !string.IsNullOrEmpty(data.TargetAgentId))
            {
                var groupMessageKey = $"group:{data.TargetAgentId}:{messageKey}";
                return await base.TriggerSingleAsync(groupMessageKey, data, wait, filePath, num);
            }

            // Original broadcast behavior (unchanged)
            return await base.TriggerSingleAsync(messageKey, data, wait, filePath, num);
        }
        /// <summary>
        /// 初始化日志模块
        /// </summary>
        /// <param name="logger"></param>
        public void InitLogger(AbsLoggerProvider? logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// 回调池
        /// 主字典的Key:事件ID
        /// 主字典的Value:这个事件所包含的所有需要回调的函数信息
        /// </summary>
        public Dictionary<string, Dictionary<string, Func<MessageDataItem?, MessageDataItem?>>> _callBackPool = new();
        /// <summary>
        /// 回调池 独立事件
        /// 主字典的Key:事件ID
        /// 主字典的Value:这个事件所包含的所有需要回调的函数信息
        /// </summary>
        public Dictionary<string, Func<MessageDataItem?, MessageDataItem?>> _singleCallBackPool = new();

        /// <summary>
        /// 注册一个事件
        /// </summary>
        /// <param name="messageKey">事件Key</param>
        /// <param name="cb">回调函数</param>
        /// <param name="cbID">注册ID</param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        public void RegistCB(string messageKey, Func<MessageDataItem?, MessageDataItem?> cb, string cbID, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            Logger?.Trace($"尝试注册回调, MessageKey:{messageKey} cbID:{cbID} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);


            if (!_callBackPool.ContainsKey(messageKey))//查看又没有这个事件注册,比如完全没有
            {
                _callBackPool.Add(messageKey, new());//我就创建这个事件给池
            }
            if (_callBackPool.TryGetValue(messageKey, out var value))//我拿到这个池子所有的需要回调的函数池
            {
                if (!value.ContainsKey(cbID))//看看有没有同ID的回调函数
                {
                    value.Add(cbID, cb);//添上
                }
            }
        }

        /// <summary>
        /// 注册一个独立事件，该类型的事件只能存在一个实例        
        /// </summary>
        /// <remarks>该类事件如果注册多次，会以最后一次为准</remarks>
        /// <param name="messageKey">事件Key</param>
        /// <param name="cb">回调函数</param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        public void RegistSingleCB(string messageKey, Func<MessageDataItem?, MessageDataItem?> cb, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0)
        {
            Logger?.Trace($"尝试注册独立事件回调, MessageKey:{messageKey} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);


            if (!_singleCallBackPool.ContainsKey(messageKey))//查看又没有这个事件注册,比如完全没有
            {
                _ = _singleCallBackPool.Remove(messageKey);//我就创建这个事件给池
            }
            _singleCallBackPool.Add(messageKey, cb);
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
            Logger?.Trace($"尝试注销回调, MessageKey:{messageKey} 发起文件:{filePath} 行号:{num}", eventID: IDESessionID);


            if (_callBackPool.TryGetValue(messageKey, out var cbs))//尝试用事件Key,获取一下这个事件Key所有的回调池
            {
                _ = cbs.Remove(cbID);//直接移除对应的回调ID
            }
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

            _ = _singleCallBackPool.Remove(messageKey);//我就创建这个事件给池
        }

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
                foreach (var item in cbs)
                {
                    tasks.Add(Task.Run(() =>
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



        /// <summary>
        /// 是否某个消息已经处理,防颤
        /// </summary>
        /// <returns></returns>
        private bool IsHandled(MessageDataItem? data)
        {
            lock (_handledMessages)
            {
                if (data is null)
                {
                    return false;
                }
                if (_handledMessages.Contains(data.MessageID))
                {
                    return true;
                }
                _handledMessages.Enqueue(data.MessageID);
                if (_handledMessages.Count > CacheMessageKeyCount)
                {
                    for (int i = 0; i < ClearMessageKeyCount; i++)
                    {
                        _ = _handledMessages.Dequeue();
                    }
                }
                return false;
            }
        }
    }
}
