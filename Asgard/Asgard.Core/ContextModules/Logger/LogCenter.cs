using Asgard.Core.ContextModules.Logger.AbsInfos;
using Asgard.Core.ContextModules.Logger.LogRealizations;
using Asgard.Core.ContextModules.Logger.Models;

namespace Asgard.Core.ContextModules.Logger
{
    /// <summary>
    /// 日志中心
    /// </summary>
    internal class LogCenter : AbsLogger, IDisposable
    {
        /// <summary>
        /// 选项
        /// </summary>
        private readonly LogOptions _options;

        /// <summary>
        /// 文件日志实例
        /// </summary>
        private readonly FileLog? _fileLogInstance;

        /// <summary>
        /// 区域号
        /// </summary>
        private readonly string? _scope;

        private readonly LoggerProvider _loggerProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="options"></param>
        /// <param name="fileLog"></param>
        /// <param name="loggerProvider"></param>
        public LogCenter(string moduleName, LogOptions options, FileLog? fileLog, LoggerProvider loggerProvider) : this(moduleName, options, fileLog, null, loggerProvider)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="options"></param>
        /// <param name="fileLog"></param>
        /// <param name="scopeName"></param>
        /// <param name="loggerProvider"></param>
        public LogCenter(string moduleName, LogOptions options, FileLog? fileLog, string? scopeName, LoggerProvider loggerProvider) : base(moduleName)
        {
            _fileLogInstance = fileLog;
            _options = options;
            _scope = scopeName;
            _loggerProvider = loggerProvider;
        }

        /// <summary>
        /// 销毁函数
        /// </summary>
        protected override void Dispose(bool flag)
        {
            if (_disposed) return;
            if (_scope is not null)
            {
                try
                {
                    _fileLogInstance?.Dispose();
                    base.Dispose(flag);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"销毁日志中心报错:{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public override void Log(LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name)
        {
            if (_options.EnableConsole && _options.MinLevel <= level)
            {
                WriteConsoleLog(level, text, eventID, exception, filePath, num, name);
            }
            if (_options.FileLog.EnableFileLog && _options.MinLevel <= level)
            {
                WriteFileLog(level, text, eventID, exception, filePath, num, name);
            }
            if (_options.DbLog.EnableDBLog && _options.MinLevel <= level)
            {
                _loggerProvider.WriteLog(_moduleName, _scope, level, text, eventID, exception, filePath, num, name);
            }
            _loggerProvider.TriggeWiteLog(_moduleName, _scope, level, text, eventID, exception, filePath, num, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="text"></param>
        /// <param name="eventID"></param>
        /// <param name="exception"></param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        /// <param name="name"></param>
        private void WriteConsoleLog(LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name)
        {
            switch (level)
            {
                case LogLevelEnum.Trace:
                    ConsoleLog.Instance.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name));
                    break;
                case LogLevelEnum.Debug:
                    ConsoleLog.Instance.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name));
                    break;
                case LogLevelEnum.Information:
                    ConsoleLog.Instance.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name), fontColor: ConsoleColor.Green);
                    break;
                case LogLevelEnum.Warning:
                    ConsoleLog.Instance.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name), ConsoleColor.Yellow, ConsoleColor.Black);
                    break;
                case LogLevelEnum.Error:
                    ConsoleLog.Instance.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name), ConsoleColor.Red, ConsoleColor.White);
                    break;
                case LogLevelEnum.Critical:
                    ConsoleLog.Instance.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name), ConsoleColor.DarkRed, ConsoleColor.White);
                    break;
                default:
                    ConsoleLog.Instance.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name), ConsoleColor.Green, ConsoleColor.Red);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="text"></param>
        /// <param name="eventID"></param>
        /// <param name="exception"></param>
        /// <param name="filePath"></param>
        /// <param name="num"></param>
        /// <param name="name"></param>
        private void WriteFileLog(LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name)
        {
            _fileLogInstance?.Write(GetLogFullString(level, text, eventID, exception, filePath, num, name));
        }


    }
}
