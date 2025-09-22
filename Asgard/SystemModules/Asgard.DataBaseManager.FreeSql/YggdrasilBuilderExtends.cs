using Asgard.Abstract;

namespace Asgard.DataBaseManager.FreeSql
{
    /// <summary>
    /// 世界之树构建器扩展
    /// </summary>
    public static class YggdrasilBuilderExtends
    {
        /// <summary>
        /// 使用FreeSql作为数据库管理器
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static YggdrasilBuilder UseFreeSqlDBManager(this YggdrasilBuilder builder)
        {

            return builder.SetDBManager((loggerProvider, nodeConfig) => new FreeSqlManager(loggerProvider, nodeConfig));
        }
    }
}
