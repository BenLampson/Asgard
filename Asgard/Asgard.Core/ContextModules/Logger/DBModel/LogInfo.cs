using FreeSql.DataAnnotations;

namespace Asgard.Core.ContextModules.Logger.DBModel
{
    /// <summary>
    /// 日志数据库表
    /// </summary>
    [Table(Name = "LogInfo")]
    public class LogInfo
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        [Column(DbType = "varchar(1024)")]
        public string ModuleName { get; set; } = "";
        /// <summary>
        /// 范围
        /// </summary>
        public string Scope { get; set; } = "";
        /// <summary>
        /// 级别
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 文本内容
        /// </summary>
        [Column(DbType = "MEDIUMTEXT")]
        public string Text { get; set; } = "";

        /// <summary>
        /// 堆栈信息
        /// </summary>
        [Column(DbType = "MEDIUMTEXT")]
        public string Stack { get; set; } = "";
        /// <summary>
        /// 事件ID
        /// </summary>
        public string EventID { get; set; } = "";
        /// <summary>
        /// 异常文本
        /// </summary>
        [Column(DbType = "MEDIUMTEXT")]
        public string Exception { get; set; } = "";
        /// <summary>
        /// 文件地址
        /// </summary>
        [Column(DbType = "varchar(1024)")]
        public string FilePath { get; set; } = "";
        /// <summary>
        /// 行号
        /// </summary>
        public int RowNumber { get; set; }
        /// <summary>
        /// 函数名
        /// </summary>
        [Column(DbType = "varchar(1024)")]
        public string MethodName { get; set; } = "";
        /// <summary>
        /// 创建时间
        /// </summary>        
        public long Created { get; set; } = DateTime.UtcNow.Ticks;
    }
}
