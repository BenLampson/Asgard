namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig
{
    /// <summary>
    /// 文件日志的配置
    /// </summary>
    public class FileLogOptions
    {
        /// <summary>
        /// 启用文件日志
        /// </summary>
        public bool EnableFileLog { get; set; } = false;
        /// <summary>
        /// 文件存放目录
        /// </summary>
        public string FolderPath { get; set; } = AppContext.BaseDirectory;

        /// <summary>
        /// 日志文件大小,单位MB
        /// </summary>
        public int MaxSize { get; set; } = 20;
    }
}
