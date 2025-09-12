using System.Threading.Channels;

using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel.LogConfig;
using Asgard.Logger.FreeSqlProvider.DBModel;

using FreeSql;

namespace Asgard.Logger.FreeSqlProvider.LogRealizations
{
    /// <summary>
    /// 数据库日志
    /// </summary>
    internal class DBLog
    {
        /// <summary>
        /// 数据库日志池
        /// </summary>
        private readonly Channel<LogInfo> _dbLogPool = Channel.CreateUnbounded<LogInfo>();

        /// <summary>
        /// 日志数据库
        /// </summary>
        private readonly IFreeSql? _logDB;

        public DBLog(DBLogOptions opt)
        {
            Console.WriteLine($"尝试创建数据库日志表.{opt.TableName}");
            _logDB = new FreeSqlBuilder().UseConnectionString((DataType)opt.DbType, opt.DbAddress).UseExitAutoDisposePool(false).Build();

            if (!_logDB.DbFirst.ExistsTable(opt.TableName))
            {
                _logDB.CodeFirst.SyncStructure(typeof(LogInfo), opt.TableName);
            }

            Console.WriteLine($"数据库日志初始化完成.{opt.TableName}");
            DBLogJob(opt);
        }


        /// <summary>
        /// 数据库日志任务
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
                        Console.WriteLine($"保存数据库日志报错. 错误信息:{ex}");
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
