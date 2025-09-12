using Asgard.Core.ContextModules.ConfigCenter.DBModels.Node;
using Asgard.Core.ContextModules.Tools;

using FreeSql.DataAnnotations;

namespace Asgard.Core.ContextModules.ConfigCenter.DBModels
{
    /// <summary>
    /// 配置信息
    /// </summary>
    [Table(Name = "asgard_system_configs")]
    [Index("uk_asgard_system_configs_name", "name asc")]
    public class SystemConfig
    {
        /// <summary>
        /// 配置主键ID
        /// </summary>
        [Column(IsPrimary = true)]
        public long ID { get; set; } = IDGen.Instance?.NextId() ?? -1;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 值
        /// </summary>
        [Column(StringLength = -1)]
        [JsonMap]
        public NodeConfig Value { get; set; } = new();
    }
}
