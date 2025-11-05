using System.Runtime.CompilerServices;

using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig;

namespace Asgard.Logger.FreeSqlProvider
{
    /// <summary>
    /// Log handler
    /// </summary>
    public class LogHandler : IDisposable
    {
        private readonly AbsLogger _logger;
        private readonly string _eventID;
        private readonly string _filePath;
        private readonly int _num;
        private readonly string _memberName;
        private readonly LogLevelEnum _enterAndExitLevel;
        /// <summary>
        /// Constructor
        /// </summary>
        public LogHandler(AbsLogger logger, LogLevelEnum enterAndExitLevel, string eventID, [CallerFilePath] string filePath = "", [CallerLineNumber] int num = 0, [CallerMemberName] string name = "")
        {
            _logger = logger;
            _eventID = eventID;
            _filePath = filePath;
            _enterAndExitLevel = enterAndExitLevel;
            _num = num;
            _memberName = name;
            SaveLog($"Enter:{_memberName}");
        }

        /// <summary>
        /// Trace log
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filePath"></param>
        /// <param name="exception"></param>
        /// <param name="num"></param>
        /// <param name="name"></param>
        public void Trace(string text,
            Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _logger.Log(LogLevelEnum.Trace, text, _eventID, exception, filePath, num, name);
        }


        /// <summary>
        /// Record debug log
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="exception">Exception</param>
        /// <param name="filePath">File name</param>
        /// <param name="name">Function name</param>
        /// <param name="num">Line number</param>
        public void Debug(string text, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _logger.Log(LogLevelEnum.Debug, text, _eventID, exception, filePath, num, name);
        }
        /// <summary>
        /// Record information log
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="exception">Exception</param>
        /// <param name="filePath">File name</param>
        /// <param name="name">Function name</param>
        /// <param name="num">Line number</param>
        public void Information(string text, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _logger.Log(LogLevelEnum.Information, text, _eventID, exception, filePath, num, name);
        }
        /// <summary>
        /// Record warning log
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="exception">Exception</param>
        /// <param name="filePath">File name</param>
        /// <param name="name">Function name</param>
        /// <param name="num">Line number</param>
        public void Warning(string text, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _logger.Log(LogLevelEnum.Warning, text, _eventID, exception, filePath, num, name);
        }

        /// <summary>
        /// Record error log
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="exception">Exception</param>
        /// <param name="filePath">File name</param>
        /// <param name="name">Function name</param>
        /// <param name="num">Line number</param>
        public void Error(string text, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _logger.Log(LogLevelEnum.Error, text, _eventID, exception, filePath, num, name);
        }


        /// <summary>
        /// Record critical log
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="exception">Exception</param>
        /// <param name="filePath">File name</param>
        /// <param name="name">Function name</param>
        /// <param name="num">Line number</param>
        public void Critical(string text, Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int num = 0,
            [CallerMemberName] string name = "")
        {
            _logger.Log(LogLevelEnum.Critical, text, _eventID, exception, filePath, num, name);
        }








        private void SaveLog(string text)
        {
            switch (_enterAndExitLevel)
            {
                case LogLevelEnum.Trace:
                    _logger.Trace(text, eventID: _eventID, exception: null, filePath: _filePath, num: _num, name: _memberName);
                    break;
                case LogLevelEnum.Debug:
                    _logger.Debug(text, eventID: _eventID, exception: null, filePath: _filePath, num: _num, name: _memberName);
                    break;
                case LogLevelEnum.Information:
                    _logger.Information(text, eventID: _eventID, exception: null, filePath: _filePath, num: _num, name: _memberName);
                    break;
                case LogLevelEnum.Warning:
                    _logger.Warning(text, eventID: _eventID, exception: null, filePath: _filePath, num: _num, name: _memberName);
                    break;
                case LogLevelEnum.Error:
                    _logger.Error(text, eventID: _eventID, exception: null, filePath: _filePath, num: _num, name: _memberName);
                    break;
                case LogLevelEnum.Critical:
                    _logger.Critical(text, eventID: _eventID, exception: null, filePath: _filePath, num: _num, name: _memberName);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~LogHandler()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose function
        /// </summary>
        public void Dispose()
        {
            SaveLog($"Exit:{_memberName}");
            GC.SuppressFinalize(this);
        }
    }
}
