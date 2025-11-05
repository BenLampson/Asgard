using System.Runtime.CompilerServices;

namespace Asgard.Tools
{
    /// <summary>
    /// 极致性能的通配符匹配器（仅支持 *）
    /// </summary>
    public static class Wildcard
    {
        /// <summary>
        /// 判断 text 是否匹配 pattern（仅支持 * 通配符）
        /// 零分配、线性时间、适用于高频热路径
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMatch(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern)
        {
            // 特殊情况：空模式
            if (pattern.Length == 0)
                return text.Length == 0;

            // 特殊情况：模式为 "*"
            if (pattern.Length == 1 && pattern[0] == '*')
                return true;

            int t = 0;           // text index
            int p = 0;           // pattern index
            int starPos = -1;    // 最近一个 '*' 在 pattern 中的位置
            int matchPos = 0;    // text 中与 '*' 匹配的起始位置

            // 主匹配循环
            while (t < text.Length)
            {
                if (p < pattern.Length && pattern[p] == '*')
                {
                    // 遇到 '*'：记录位置，跳过它
                    starPos = p++;
                    matchPos = t;
                }
                else if (p < pattern.Length && text[t] == pattern[p])
                {
                    // 字符匹配，双指针前进
                    t++;
                    p++;
                }
                else if (starPos != -1)
                {
                    // 不匹配，但有 '*' 可回溯：让 '*' 多吃一个字符
                    p = starPos + 1;
                    t = ++matchPos;
                }
                else
                {
                    // 无 '*' 可回溯，匹配失败
                    return false;
                }
            }

            // 跳过 pattern 末尾所有连续的 '*'
            while (p < pattern.Length && pattern[p] == '*')
            {
                p++;
            }

            return p == pattern.Length;
        }

        /// <summary>
        /// 字符串重载（仅用于方便调用，内部转为 Span）
        /// 注意：此方法会产生 Span 封装开销（但无堆分配）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMatch(string? text, string? pattern)
        {
            if (pattern is null)
            {
                ThrowArgumentNullException(nameof(pattern));
            }

            if (text is null && pattern is not null)
            {
                return pattern.Length == 0 || (pattern.Length == 1 && pattern[0] == '*');
            }

            return IsMatch(text.AsSpan(), pattern.AsSpan());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentNullException(string paramName)
            => throw new ArgumentNullException(paramName);
    }
}
