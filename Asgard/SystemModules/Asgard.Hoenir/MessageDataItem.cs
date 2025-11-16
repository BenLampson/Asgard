namespace Asgard.Hoenir
{
    /// <summary>
    /// 消息数据项，封装消息的基本信息
    /// </summary>
    public class MessageDataItem(string? source = null, string? to = null)
    {
        /// <summary>
        /// 消息来源
        /// </summary>
        public string Source { get; private set; } = source ?? "";
        /// <summary>
        /// 目标接收者，通常为接收方ID，可为 null
        /// </summary>
        public string? To { get; private set; } = to;
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MessageID { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 消息内容
        /// </summary>
        public object? Data { get; set; }
        /// <summary>
        /// 消息时间戳（Ticks）
        /// </summary>
        public long TimeStamp { get; init; } = DateTime.Now.Ticks;
        /// <summary>
        /// 来源文件路径
        /// </summary>
        public string FromFile { get; set; } = string.Empty;
        /// <summary>
        /// 行号
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// 消息头，存放一些通用元数据
        /// </summary>
        public Dictionary<string, object> Header { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 获取消息内容并尝试类型转换，失败则返回默认值
        /// </summary>
        public T? GetData<T>()
        {
            if (Data is T res)
            {
                return res;
            }
            return default;
        }
        /// <summary>
        /// 获取消息内容并尝试类型转换，返回是否转换成功
        /// </summary>
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
