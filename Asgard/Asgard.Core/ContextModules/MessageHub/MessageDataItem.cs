namespace Asgard.Core.ContextModules.MessageHub
{
    /// <summary>
    /// 消息中心消息体
    /// </summary>
    public class MessageDataItem
    {
        /// <summary>
        /// 构造一个新的消息
        /// </summary>
        /// <param name="source">来源,系统会在一定窗口期自动过滤重复消息源</param>
        public MessageDataItem(string? source = null)
        {
            Source = source ?? "";
        }
        /// <summary>
        /// 由谁发起的
        /// </summary>
        public string Source { get; private set; }
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MessageID { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 内容
        /// </summary>
        public object? Data { get; set; }
        /// <summary>
        /// 发起时间
        /// </summary>
        public long TimeStamp { get; init; } = DateTime.Now.Ticks;
        /// <summary>
        /// 哪个文件调用的
        /// </summary>
        public string FromFile { get; set; } = string.Empty;
        /// <summary>
        /// 第几行
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// 头消息可以传递一些普通参数
        /// </summary>
        public Dictionary<string, object> Header { get; set; } = new();

        /// <summary>
        /// 获取数据,如果是这个类型,则转换,如果不是则为空
        /// </summary>
        /// <typeparam name="T">想要转换到的类型</typeparam>
        /// <returns></returns>
        public T? GetData<T>()
        {
            if (Data is T res)
            {
                return res;
            }
            return default;
        }
        /// <summary>
        /// 获取数据,如果是这个类型,则转换,如果不是则为空
        /// </summary>
        /// <typeparam name="T">想要转换到的类型</typeparam>
        /// <returns></returns>
        public bool TryGetData<T>(out T? data)
        {
            data = default;
            if (Data is T res)
            {
                data = res;
                return true;
            }
            return false;
        }

    }
}
