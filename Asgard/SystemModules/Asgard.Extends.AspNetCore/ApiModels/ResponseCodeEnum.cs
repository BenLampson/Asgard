using System.ComponentModel;

namespace Asgard.Extends.AspNetCore.ApiModels
{
    /// <summary>
    /// 响应Code
    /// </summary>
    [Flags]
    public enum ResponseCodeEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 0,
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error = 1,
        /// <summary>
        /// 系统内部
        /// </summary>
        [Description("系统内部")]
        System = 2,
        /// <summary>
        /// 逻辑错误
        /// </summary>
        [Description("逻辑错误")]
        Logic = 4,
        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        Parameter = 8,
        /// <summary>
        /// 网络错误
        /// </summary>
        [Description("网络错误")]
        Network = 16
    }
}
