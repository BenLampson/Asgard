using System.Collections.Concurrent;

using Asgard.Core.ContextModules.Logger.AbsInfos;
using Asgard.Core.ContextModules.Logger.DBModel;
using Asgard.Core.ContextModules.Logger.LogRealizations;
using Asgard.Core.ContextModules.Logger.Models;

namespace Asgard.Core.ContextModules.Logger
{
    /// <summary>
    /// 容器日志服务
    /// </summary>
    public class LoggerProvider : AbsLoggerProvider
    {


        /// <summary>
        /// 当发生了消息写入时,订阅这个来自己做实现
        /// </summary>
        public event Action<(string moduleName, string? scope, LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name)>? OnWriteMessage;


        /// <summary>
        /// 数据库日志
        /// </summary>
        private DBLog? DBLogInstance { get; set; }

        /// <summary>
        /// 日志的实例缓存
        /// </summary>
        private readonly ConcurrentDictionary<string, AbsLogger> _loggerPool = new();

        /// <summary>
        /// 文件日志对象
        /// </summary>
        private readonly FileLog? _fileLogInstance;

        /// <summary>
        /// 日志的配置信息
        /// </summary>
        private readonly LogOptions _options = new();


        /// <summary>
        /// 创建一个日志器
        /// </summary>
        /// <param name="categoryName">分类/模块名称</param>
        /// <returns></returns>
        public override AbsLogger CreateLogger(string categoryName)
        {
            return _loggerPool.GetOrAdd(categoryName, (key) =>
            {
                return new LogCenter(categoryName, _options, _fileLogInstance, this);
            });

        }

        /// <summary>
        /// 根据类型创建一个日志器
        /// </summary> 
        /// <returns></returns>
        public override AbsLogger CreateLogger<T>()
        {
            return CreateLogger(typeof(T).FullName ?? "Error type");
        }

        /// <summary>
        /// 创建一个局部日志器
        /// </summary>
        /// <param name="categoryName">分类/模块名称</param>
        /// <param name="scopeName">局部自定义名称</param>
        /// <returns></returns>
        public override AbsLogger? CreateScopeLogger(string categoryName, string scopeName)
        {
            FileLog? fileLog = null;
            if (_options.FileLog.EnableFileLog)
            {
                fileLog = new FileLog(new FileLogOptions() { FolderPath = _options.FileLog.FolderPath }, scopeName);
            }
            return new LogCenter(categoryName, _options, fileLog, scopeName, this);

        }

        /// <summary>
        /// 根据类型创建一个局部日志器
        /// </summary> 
        /// <param name="scopeName">局部自定义名称</param>
        /// <returns></returns>
        public override AbsLogger? CreateScopeLogger<T>(string scopeName)
        {
            return CreateScopeLogger(typeof(T).FullName ?? "Error type", scopeName);
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">日志参数</param>
        public LoggerProvider(LogOptions options)
        {
            _options = options;
            if (options.FileLog.EnableFileLog)
            {
                _fileLogInstance = new FileLog(options.FileLog);
            }
            if (_options.DbLog.EnableDBLog)
            {
                DBLogInstance = new DBLog(_options.DbLog);
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="scope">范围</param>
        /// <param name="level">级别</param>
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        internal void WriteLog(string moduleName, string? scope, LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name)
        {
            try
            {
                DBLogInstance?.TryWrite(new LogInfo()
                {
                    EventID = eventID ?? "",
                    Exception = exception?.Message ?? "",
                    Level = (int)level,
                    ModuleName = moduleName,
                    Text = text,
                    Scope = scope ?? "",
                    FilePath = filePath,
                    Stack = exception?.StackTrace ?? "",
                    MethodName = name,
                    RowNumber = num
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"调用写入消息出错:{ex.Message}");
            }
        }

        /// <summary>
        /// 触发一次写入日志
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="scope">范围</param>
        /// <param name="level">级别</param>
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        internal void TriggeWiteLog(string moduleName, string? scope, LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name)
        {
            OnWriteMessage?.Invoke((moduleName, scope, level, text, eventID, exception, filePath, num, name));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }


        /// <summary>
        /// 创建一个普通的Console的Log提供器
        /// </summary>
        /// <returns></returns>
        public static LoggerProvider CreateConsoleLogProvider(LogLevelEnum logLevel)
        {
            var consoleLoggerProvider = new LoggerProvider(new LogOptions()
            {
                EnableConsole = true,
                MinLevel = logLevel
            });
            return consoleLoggerProvider;
        }
    }
}
