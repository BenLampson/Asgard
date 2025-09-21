using System.Diagnostics;

namespace Asgard.Extends.AspNetCore.JSCreatorNextVersion
{
    /// <summary>
    /// 控制器的模型类型信息
    /// </summary>
    [DebuggerDisplay("{RawType.FullName}")]
    public class ControllerModelTypeInfo
    {
        /// <summary>
        /// 原始模型的类型
        /// </summary>
        public Type RawType { get; set; }

        /// <summary>
        /// 是否是返回
        /// </summary>
        public bool IsResult { get; set; }
        /// <summary>
        /// 是否是枚举
        /// </summary>
        public bool IsEnum { get; set; }
        /// <summary>
        /// 是否是分页返回
        /// </summary>
        public bool IsPageResult { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Notic { get; set; }

        /// <summary>
        /// 对应的属性列表 ,key 属性跟他的类型,value
        /// </summary>
        public List<(string name, string type, string description, bool nullAble, bool isArray, bool isEnum)> Properties { get; set; } = new();


    }
}
