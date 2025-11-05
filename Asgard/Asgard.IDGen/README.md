# Asgard.IDGen

## 概述
Asgard.IDGen 是一个基于雪花算法（Snowflake）实现的唯一ID生成器库。该库适用于需要生成全局唯一标识符的应用场景，特别适用于分布式系统中。

## 主要功能
- 使用雪花算法生成唯一ID。
- 支持自定义工作节点ID和数据中心ID。
- 提供线程安全的ID生成方法。

## 安装
可以通过NuGet包管理器安装Asgard.IDGen：

```bash
dotnet add package Asgard.IDGen
```

## 使用示例
以下是一个简单的使用示例，展示如何初始化ID生成器并生成唯一ID：

```csharp
using Asgard.IDGen;

class Program
{
    static void Main(string[] args)
    {
        // 初始化ID生成器，设置工作节点ID和数据中心ID
        SnowflakeIDGen.Init(workerID: 1, datacenter: 1);

        // 获取ID生成器实例
        var idGen = SnowflakeIDGen.Instance;

        // 生成唯一ID
        long uniqueId = idGen.NextId();

        Console.WriteLine($"Generated Unique ID: {uniqueId}");
    }
}
```

## 类说明
- **SnowflakeIDGen**: 实现雪花算法的核心类，提供ID生成功能。
- **IdWorkSystem**: 内部静态类，用于获取当前时间戳。
- **DisposableAction**: 内部类，用于执行可释放操作。

## 注意事项
- 在使用`NextId`方法之前，必须先调用`Init`方法进行初始化。
- 工作节点ID和数据中心ID的范围分别为0到31。

## 贡献
欢迎贡献代码和提出改进建议。请参考[贡献指南](CONTRIBUTING.md)。

## 许可证
本项目采用MIT许可证。详情请参见[LICENSE](LICENSE)文件。