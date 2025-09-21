//using Asgard.Abstract;
//using Asgard.Abstract.Cache;
//using Asgard.ConfigCenter.DBModels;

//namespace Asgard.ConfigCenter
//{
//    /// <summary>
//    /// 系统配置信息
//    /// </summary>
//    public class SystemConfigInfo
//    {

//        /// <summary>
//        /// 获取数据
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public static List<SystemConfig>? GetConfig(AsgardContext context)
//        {
//            if (context.Cache is null)
//            {
//                if (context.DB.DefaultDB() is null)
//                {
//                    return new List<SystemConfig>();
//                }
//                var allConfigs = context.DB.Default.Select<SystemConfig>().ToList();
//                return allConfigs;
//            }
//            return context.Cache.GetOrSet($"SystemConfigCenter", () =>
//              {
//                  if (context.DB.Default is null)
//                  {
//                      return new List<SystemConfig>();
//                  }
//                  var allConfigs = context.DB.Default.Select<SystemConfig>().ToList();


//                  return allConfigs;
//              }, new CacheItemSettings()
//              {
//                  AbsoluteExpiration = DateTime.Now.AddYears(1)
//              });

//        }

//        /// <summary>
//        /// 刷新缓存
//        /// </summary>
//        /// <param name="context"></param>
//        public static void RefreshCache(AsgardContext context)
//        {
//            if (context.Cache is null)
//            {
//                return;
//            }
//            context.Cache.Remove("SystemConfigCenter");
//            _ = GetConfig(context);
//        }



//        /// <summary>
//        /// 根据路径获取数据 包含了子集
//        /// </summary>
//        /// <param name="context">上下文</param>
//        /// <param name="pointName"></param>
//        /// <returns></returns>
//        public static SystemConfig? GetByNodeName(AsgardContext<IFreeSql> context, string pointName)
//        {
//            var data = GetConfig(context);
//            if (data is null)
//            {
//                return default;
//            }
//            SystemConfig? current = data.FirstOrDefault(item => item.Name.Equals(pointName, StringComparison.OrdinalIgnoreCase));

//            if (current is null)
//            {
//                return default;
//            }
//            return current;
//        }
//    }
//}
