namespace Asgard.ConfigCenter.TCPPackage
{
    /// <summary>
    /// 配置中心操作枚举
    /// </summary>
    public enum ConfigCenterOperationEnum
    {
        /// <summary>
        /// 根据路径获取这个节点以及它之下的内容
        /// </summary>
        GetByPointName = 0,
        /// <summary>
        /// 让服务重新拉取缓存数据
        /// </summary>
        ReloadData = 1,
        /// <summary>
        /// 节点的配置发生了改变
        /// </summary>
        PointConfigChanged = 2,
        /// <summary>
        /// 设置我的节点名称
        /// </summary>
        SetMyPointName = 3
    }
}
