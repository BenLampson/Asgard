using NCalc;

namespace Asgard.Core.ContextModules.Tools.CalcTools
{
    /// <summary>
    /// 计算工具实例对象,线程非安全
    /// </summary>
    ///<threadsafety>该类不是线程安全的。</threadsafety>
    public class CalcToolsInstance
    {
        /// <summary>
        /// 表达式对象
        /// </summary>
        private readonly Expression _expression;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="expiress"></param>
        public CalcToolsInstance(string expiress)
        {
            _expression = new Expression(expiress);
        }

        /// <summary>
        /// 是否有错误
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool HasError(out string error)
        {
            var hasError = _expression.HasErrors();
            error = _expression.Error?.Message ?? "";
            return hasError;
        }

        /// <summary>
        /// 执行并获取结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T? GetResult<T>(Dictionary<string, object> parameters)
        {
            try
            {
                _expression.Parameters.Clear();
                foreach (var item in parameters)
                {
                    _expression.Parameters[item.Key] = item.Value;
                }
                var res = _expression.Evaluate();
                if (res is T obj)
                {
                    return obj;
                }
                return default;
            }
            catch
            {

                return default;
            }
        }
    }
}
