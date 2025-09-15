namespace Asgard.Extends.AspNetCore.ApiModels
{
    /// <summary>
    /// 响应基础类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataResponse<T>
    {
        /// <summary>
        /// 响应状态
        /// </summary>
        public ResponseCodeEnum Code { get; set; }
        /// <summary>
        /// 自定义消息
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// 携带的数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int DataCount { get; set; }
    }
}
