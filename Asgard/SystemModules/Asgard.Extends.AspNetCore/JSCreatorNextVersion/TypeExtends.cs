namespace Asgard.Extends.AspNetCore.JSCreatorNextVersion
{
    internal static class TypeExtends
    {
        /// <summary>
        ///  Nullable类型
        ///  从Freesql拿的
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type that) => that == null ? false : (that.IsArray == false && that.FullName?.StartsWith("System.Nullable`1[") == true);
        /// <summary>
        /// 数组或者列表
        /// 也是从Freesql拿的
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool IsArrayOrList(this Type that) => that == null ? false : (that.IsArray || typeof(IList<>).IsAssignableFrom(that));
    }
}
