using System.Reflection;
using System.Runtime.Loader;

namespace Asgard.Plugin
{
    /// <summary>
    /// 程序集加载器
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// 基础文件夹路径地址
        /// </summary>
        private readonly string _basePath;
        private readonly AssemblyLoadContext _context;
        /// <summary>
        /// 程序集对象
        /// </summary>
        public Assembly Assembly { get; init; }

        /// <summary>
        /// 
        /// </summary> 
        /// <param name="dllFilePath">dll文件地址</param> 
        public AssemblyLoader(string dllFilePath)
        {
            var dllFile = new FileInfo(dllFilePath);
            if (!dllFile.Exists || dllFile.DirectoryName is null)
            {
                throw new FileNotFoundException($"类库文件[{dllFilePath}],不存在.");
            }
            _basePath = dllFile.DirectoryName;
            _context = new AssemblyLoadContext(dllFilePath);
            _context.Resolving += Context_Resolving;
            using var stream = File.OpenRead(dllFilePath);
            Assembly = _context.LoadFromStream(stream);
        }
        /// <summary>
        /// 加载依赖文件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private Assembly? Context_Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            string expectedPath = Path.Combine(_basePath, assemblyName.Name + ".dll"); ;
            if (File.Exists(expectedPath))
            {
                try
                {
                    using FileStream stream = File.OpenRead(expectedPath);
                    return context.LoadFromStream(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载[{expectedPath}]出错: {ex}");
                }
            }
            else
            {
                Console.WriteLine($"文件[{expectedPath}]不存在.");
            }
            return null;
        }
    }
}