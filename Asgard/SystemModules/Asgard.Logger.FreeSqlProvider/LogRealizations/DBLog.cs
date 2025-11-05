using System.Threading.Channels;

using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig;
using Asgard.Logger.FreeSqlProvider.DBModel;

using FreeSql;

namespace Asgard.Logger.FreeSqlProvider.LogRealizations
{
    /// <summary>
    /// Database log
    /// </summary>
    internal class DBLog
    {
        /// <summary>
        /// Database log pool
        /// </summary>
        private readonly Channel<LogInfo> _dbLogPool = Channel.CreateUnbounded<LogInfo>();

        /// <summary>
        /// Log database
        /// </summary>
        private readonly IFreeSql? _logDB;

        public DBLog(DBLogOptions opt)
        {
            Console.WriteLine($"Attempting to create database log table.{opt.TableName}");
            _logDB = new FreeSqlBuilder().UseConnectionString((DataType)opt.DbType, opt.DbAddress).UseExitAutoDisposePool(false).Build();

            if (!_logDB.DbFirst.ExistsTable(opt.TableName))
            {
                _logDB.CodeFirst.SyncStructure(typeof(LogInfo), opt.TableName);
            }

            Console.WriteLine($"Database log initialization complete.{opt.TableName}");
            DBLogJob(opt);
        }


        /// <summary>
        /// Database log task
        /// </summary>
        private void DBLogJob(DBLogOptions opt)
        {
            if (_logDB is null)
            {
                return;
            }
            _ = Task.Factory.StartNew(async () =>
            {
                var reader = _dbLogPool.Reader;
                while (true)
                {
                    var asd = await reader.WaitToReadAsync();
                    var count = reader.Count;
                    var items = new List<LogInfo>();
                    while (items.Count < count)
                    {
                        items.Add(await reader.ReadAsync());
                    }
                    try
                    {
                        if (_logDB.Select<LogInfo>().AsTable((_, _) => opt.TableName).Count() > 1_000_000)
                        {
                            var min = _logDB.Select<LogInfo>().AsTable((_, _) => opt.TableName).Skip(1_000).First();
                            _ = _logDB.Delete<LogInfo>().AsTable((_) => opt.TableName).Where(item => item.Created <= min.Created).ExecuteAffrows();

                        }
                        _ = _logDB.Insert(items).AsTable((_) => opt.TableName).ExecuteAffrows();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Save database log error. Error info:{ex}");
                        continue;
                    }
                    await Task.Delay(100);
                }

            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// DBLogInfo
        /// </summary>
        /// <param name="logInfo"></param>
        public void TryWrite(LogInfo logInfo)
        {
            _ = _dbLogPool.Writer.TryWrite(logInfo);
        }
    }
}
