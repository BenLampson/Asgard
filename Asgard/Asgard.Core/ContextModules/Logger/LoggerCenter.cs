using Asgard.Core.ContextModules.Logger.AbsInfos;

namespace Asgard.Core.ContextModules.Logger
{
    /// <summary>
    /// 泛型日志实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoggerCenter<T> : AbsLogger<T> where T : class
    {
        /// <summary>
        /// 构造函数
        /// </summary> 
        public LoggerCenter(string moduleName, LoggerProvider provider) : base(provider.CreateLogger(moduleName))
        {
        }

        /// <summary>
        /// 默认构造函数,用类型反推
        /// </summary>
        public LoggerCenter(LoggerProvider provider) : this(typeof(T).FullName ?? "错误的类型", provider)
        {
        }
    }
}
