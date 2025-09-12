using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig;

namespace Asgard.Logger.FreeSqlProvider.LogRealizations
{
    /// <summary>
    /// 文件日志写入器
    /// </summary>
    internal class FileLog : IDisposable
    {
        private StreamWriter? _targetFile;
        private readonly DirectoryInfo _logFolderInfo;
        private readonly int _logSize = 20;
        private string _currentFileName;
        private readonly string? _scopeName;

        /// <summary>
        /// 文件日志
        /// </summary>
        public FileLog(FileLogOptions options) : this(options, null)
        {
        }


        /// <summary>
        /// 文件日志
        /// </summary>
        public FileLog(FileLogOptions options, string? scopeName)
        {
            _logSize = options.MaxSize;
            _logFolderInfo = Directory.CreateDirectory(options.FolderPath);
            _scopeName = scopeName;
            _currentFileName = GetFileName();
            SetTargetFile();
        }


        private string GetFileName()
        {
            return $"{DateTime.Now:yyyy_MM_dd}{(_scopeName == null ? "" : $"_{_scopeName}")}.SSSLog";
        }

        /// <summary>
        /// 设置目标文件
        /// </summary>
        private void SetTargetFile()
        {
            try
            {
                if (_targetFile is not null)
                {
                    _targetFile.Close();
                    _targetFile.Dispose();
                }
                _currentFileName = GetFileName();
                var targetFile = Path.Combine(_logFolderInfo.FullName, _currentFileName);
                _targetFile = new FileInfo(targetFile).AppendText();
            }
            catch (Exception ex)
            {
                throw new Exception($"Set file log error:{ex.Message}");
            }
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        private void BackFile()
        {
            var info = Path.Combine(_logFolderInfo.FullName, $"{_currentFileName}.bak{Directory.GetFiles(_logFolderInfo.FullName, $"{_currentFileName}.bak*").Length}");
            File.Move(Path.Combine(_logFolderInfo.FullName, _currentFileName), info);
        }

        /// <summary>
        /// 写入
        /// </summary> 
        public void Write(string text)
        {
            lock (this)
            {
                if (_targetFile is null)
                {
                    Console.WriteLine($"无法打开或写入文件:{_currentFileName}");
                    return;
                }

                if (!_currentFileName.Equals(GetFileName()))
                {
                    SetTargetFile();
                }
                if (_targetFile.BaseStream.Length > 1024 * 1024 * _logSize)
                {

                    _targetFile?.Close();
                    _targetFile?.Dispose();
                    BackFile();
                    SetTargetFile();
                }
                if (_targetFile is null)
                {
                    Console.WriteLine($"无法打开或写入文件:{_currentFileName}");
                    return;
                }
                _targetFile.WriteLine(text);
                _targetFile.Flush();
            }
        }

        public void Dispose()
        {
            try
            {
                _targetFile?.Close();
                _targetFile?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
