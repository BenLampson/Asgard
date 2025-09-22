using Asgard.Abstract;

namespace Asgard.Logger.FreeSqlProvider
{
    /// <summary>
    /// 世界之树扩展
    /// </summary>
    public static class YggdrasilBuilderExtends
    {
        public static YggdrasilBuilder UseFreeSqlLogger(this YggdrasilBuilder builder)
        {
            return builder.SetLoggerProvider((nodeConfig) => new LoggerProvider(nodeConfig.SystemLog));
        }
    }
}
