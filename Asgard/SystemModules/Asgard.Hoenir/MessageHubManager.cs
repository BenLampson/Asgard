using System.Collections.Concurrent;

using Asgard.Abstract.Logger;

namespace Asgard.Hoenir
{
    /// <summary>
    /// 消息中心管理器
    /// </summary>
    public partial class MessageHubManager
    {
        /// <summary>
        /// 饿汉单例
        /// </summary>
        public static readonly MessageHubManager Instance = new();
        /// <summary>
        /// Global SessionID
        /// </summary>
        public static string GlobalSessionID { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 记录详细信息
        /// </summary>
        public static bool LogDetailInfo { get; set; }

        /// <summary>
        /// 处理过的消息
        /// </summary>
        private readonly ConcurrentQueue<string> _handledMessages = new ConcurrentQueue<string>();

        /// <summary>
        /// 日志组件
        /// </summary>
        private AbsLoggerProvider? _logger;
        /// <summary>
        /// messageHub日志提供器
        /// </summary>
        internal AbsLogger? Logger { get => _logger?.CreateLogger<MessageHubManager>(); }
        /// <summary>
        /// 缓冲区缓冲的重复Key个数
        /// </summary>
        public int CacheMessageKeyCount { get; set; } = 100_000;
        /// <summary>
        /// 每次满了以后清除的个数
        /// </summary>
        public int ClearMessageKeyCount { get; set; } = 1_000;
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
        public ConcurrentDictionary<string, ConcurrentDictionary<string, Func<MessageDataItem?, Task<MessageDataItem?>>>> _callBackPool = new();
        /// <summary>
        /// 回调池 独立事件
        /// 主字典的Key:事件ID
        /// 主字典的Value:这个事件所包含的所有需要回调的函数信息
        /// </summary>
        public ConcurrentDictionary<string, Func<MessageDataItem?, Task<MessageDataItem?>>> _singleCallBackPool = new();


        /// <summary>
        /// 是否某个消息已经处理,防颤
        /// </summary>
        /// <returns></returns>
        private bool IsHandled(MessageDataItem? data)
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
                    _ = _handledMessages.TryDequeue(out _);
                }
            }
            return false;
        }
    }
}
