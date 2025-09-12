using FreeSql.DataAnnotations;

namespace Asgard.Core.ContextModules.DB
{
    /// <summary>
    /// 数据库模型基础类
    /// </summary>
    public abstract class DBModelBase
    {

        /// <summary>
        /// 创建时间
        /// </summary> 
        public DateTime Created { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary> 
        public DateTime Modified { get; set; } = DateTime.Now;
        /// <summary>
        /// 创建人
        /// </summary>
        [Column(Name = "Created_By")]
        public long CreatedBy { get; set; } = -1;
        /// <summary>
        /// 修改人
        /// </summary>
        [Column(Name = "Modified_By")]
        public long ModifiedBy { get; set; } = -1;
    }
}
