namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig
{
    /// <summary>
    /// 日志选项
    /// </summary>
    public class LogOptions
    {

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevelEnum MinLevel { get; set; } = LogLevelEnum.Trace;
        /// <summary>
        /// 文件日志  
        /// </summary>
        public FileLogOptions FileLog { get; set; } = new FileLogOptions();

        /// <summary>
        /// 控制台日志 默认开启,但是性能很堪忧
        /// </summary>
        public bool EnableConsole { get; set; } = false;
        /// <summary>
        /// 数据库日志
        /// </summary>

        public DBLogOptions DbLog { get; set; } = new DBLogOptions();
    }
}
