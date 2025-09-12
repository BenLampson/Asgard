namespace Asgard.Abstract.Models.AsgardConfig.ChildConfigModel
{
    /// <summary>
    /// redis的配置信息
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Address { get; set; } = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// 默认存储数据库
        /// </summary>
        public int DefaultDataBase { get; set; } = 1;
        /// <summary>
        /// 写入缓冲区大小
        /// </summary>
        public int WriteBuffer { get; set; } = 10240;
        /// <summary>
        /// 连接池大小
        /// </summary>
        public int PoolSize { get; set; } = 100;

        /// <summary>
        /// 获取redis连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnStr()
        {
            return $"{Address},password={Password},defaultDatabase={DefaultDataBase},ssl=false,writeBuffer={WriteBuffer},poolsize={PoolSize}";
        }
    }
}
