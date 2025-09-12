using System.Runtime.CompilerServices;
using System.Text;

using Asgard.Core.ContextModules.Logger.Models;

namespace Asgard.Core.ContextModules.Logger.AbsInfos
{
    /// <summary>
    /// 容器日志抽象
    /// </summary> 
    public abstract class AbsLogger : IDisposable
    {
        /// <summary>
        /// 是否已经销毁
        /// </summary>
        protected bool _disposed = false;
        /// <summary>
        /// 模块名称
        /// </summary>
        protected readonly string _moduleName;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        public AbsLogger(string moduleName)
        {
            _moduleName = moduleName;
        }

        /// <summary>
        /// 销毁函数
        /// </summary>
        /// <param name="flag">是否是用户主动调用</param>
        protected virtual void Dispose(bool flag)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
        }

        /// <summary>
        /// 销毁函数
        /// </summary> 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 记录追踪日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Trace(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            Log(LogLevelEnum.Trace, text, eventID, exception, filePath, num, name);
        }



        /// <summary>
        /// 记录调试日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Debug(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            Log(LogLevelEnum.Debug, text, eventID, exception, filePath, num, name);
        }
        /// <summary>
        /// 记录信息日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Information(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            Log(LogLevelEnum.Information, text, eventID, exception, filePath, num, name);
        }
        /// <summary>
        /// 记录警告日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Warning(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            Log(LogLevelEnum.Warning, text, eventID, exception, filePath, num, name);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Error(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            Log(LogLevelEnum.Error, text, eventID, exception, filePath, num, name);
        }


        /// <summary>
        /// 记录危险日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Critical(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            Log(LogLevelEnum.Critical, text, eventID, exception, filePath, num, name);
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
        public abstract void Log(LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name);


        /// <summary>
        /// 获取完整的日志记录信息
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        protected string GetLogFullString(LogLevelEnum level, string text, string? eventID, Exception? exception, string filePath, int num, string name)
        {
            var sb = new StringBuilder();
            _ = sb.AppendFormat("UTC时间:{0}\r\n", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            _ = sb.AppendFormat("级别:{0}\r\n", Enum.GetName(typeof(LogLevelEnum), level));
            _ = sb.AppendFormat("模块:{0}\r\n", _moduleName);
            _ = sb.AppendFormat("事件ID:{0}\r\n", eventID);
            if (exception is not null)
            {
                _ = sb.AppendFormat("错误:{0}\r\n", exception != null ? exception.Message : string.Empty);
                _ = sb.AppendFormat("错误堆栈:{0}\r\n", exception != null ? exception.StackTrace : string.Empty);
            }
            _ = sb.AppendFormat("对应文件:{0}\r\n", filePath ?? string.Empty);
            _ = sb.AppendFormat("对应行:{0}\r\n", num);
            _ = sb.AppendFormat("对应函数:{0}\r\n", name ?? string.Empty);
            _ = sb.AppendFormat("{0}\r\n", text);
            _ = sb.AppendLine();
            return sb.ToString();
        }


    }
    /// <summary>
    /// 容器日志抽象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbsLogger<T> where T : class
    {
        /// <summary>
        /// 实际的日志实例
        /// </summary>
        public readonly AbsLogger _loggerInstance;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志具体实例</param>
        protected AbsLogger(AbsLogger logger)
        {
            _loggerInstance = logger;
        }


        /// <summary>
        /// 记录追踪日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Trace(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _loggerInstance.Log(LogLevelEnum.Trace, text, eventID, exception, filePath, num, name);
        }



        /// <summary>
        /// 记录调试日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Debug(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _loggerInstance.Log(LogLevelEnum.Debug, text, eventID, exception, filePath, num, name);
        }
        /// <summary>
        /// 记录信息日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Information(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _loggerInstance.Log(LogLevelEnum.Information, text, eventID, exception, filePath, num, name);
        }
        /// <summary>
        /// 记录警告日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Warning(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _loggerInstance.Log(LogLevelEnum.Warning, text, eventID, exception, filePath, num, name);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Error(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _loggerInstance.Log(LogLevelEnum.Error, text, eventID, exception, filePath, num, name);
        }


        /// <summary>
        /// 记录危险日志
        /// </summary>        
        /// <param name="text">文本</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="exception">异常</param>
        /// <param name="filePath">文件名</param>
        /// <param name="name">函数名</param>
        /// <param name="num">行数</param>
        public void Critical(string text, string? eventID = null, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _loggerInstance.Log(LogLevelEnum.Critical, text, eventID, exception, filePath, num, name);
        }
    }
}
