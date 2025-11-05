namespace Asgard.Abstract.Models.AsgardUserInfo
{
    /// <summary>
    /// 容器用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UID { get; set; }

        /// <summary>
        /// 当前正在使用的商户ID
        /// </summary>
        public long CurrentTID { get; set; }

        /// <summary>
        /// 角色信息
        /// </summary>
        public string[] Roles { get; set; } = Array.Empty<string>();
        /// <summary>
        /// 岗位信息
        /// </summary>
        public string[] Posts { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 当前商户的数据库信息
        /// </summary>
        public List<TDBItem> TDBInfo { get; set; } = new();

        /// <summary>
        /// 用户自定义的信息,仅限少量的自定义数据,不能存储重要数据哦!
        /// </summary>
        public Dictionary<string, string> CustomInfo { get; set; } = new();

    }
}
