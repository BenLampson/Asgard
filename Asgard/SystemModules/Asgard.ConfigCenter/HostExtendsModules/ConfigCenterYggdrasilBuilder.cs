using System.Net;
using System.Text.Json;

using Asgard.Abstract;
using Asgard.ConfigCenter.DBModels;
using Asgard.Extends.Json;

namespace Asgard.ConfigCenter.HostExtendsModules
{
    /// <summary>
    /// 配置中心的世界之树扩展
    /// </summary>
    public static class ConfigCenterYggdrasilBuilder
    {
        /// <summary>
        /// 从默认文件中加载节点配置,默认是应用程序目录下的appsettings.json,如果没有则会生成一个范本文件
        /// </summary>
        /// <param name="builder">世界之树建造者</param>
        /// <returns>世界之树建造者</returns>
        public static YggdrasilBuilder UseNodeConfigFromFile(this YggdrasilBuilder builder)
        {
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.json")))
            {
                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), JsonSerializer.Serialize(new SystemConfig()));
                throw new ArgumentException($"配置文件不存在,系统现在将在程序目录下生成一个范本,请填写!");
            }

            return builder;
        }
        /// <summary>
        /// 从文件中加载节点配置
        /// </summary>
        /// <param name="builder">世界之树建造者</param>
        /// <param name="filePath">配置文件路径</param>
        /// <returns>世界之树建造者</returns>
        public static YggdrasilBuilder UseNodeConfigFromFile(this YggdrasilBuilder builder, string filePath)
        {
            var configStr = File.ReadAllText(filePath);
            var tmpConfig = JsonSerializer.Deserialize<SystemConfig>(configStr, CommonSerializerOptions.CamelCaseChineseNameCaseInsensitive);
            if (tmpConfig is null || string.IsNullOrWhiteSpace(tmpConfig.Name))
            {
                throw new ArgumentException($"点位名称为空!");
            }
            if (string.IsNullOrWhiteSpace(tmpConfig.Value.ConfigCenter.ConfigCenter)
                || !IPAddress.TryParse(tmpConfig.Value.ConfigCenter.ConfigCenter, out var _)
                )
            {
                throw new ArgumentException($"配置中心的地址错误!当前我获取到的是:{tmpConfig.Value.ConfigCenter.ConfigCenter ?? ""}");
            }

            return builder.SetNodeConfig(tmpConfig.Value);
        }



    }
}
