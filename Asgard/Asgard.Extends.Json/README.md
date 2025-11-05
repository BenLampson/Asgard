# Asgard.Extends.Json

## 简介
Asgard.Extends.Json 是 Asgard 框架的一部分，提供了一些常用的 `System.Text.Json` 扩展功能，包括自定义的转换器（Converters）。

## 功能特性
- **公共序列化配置**：定义了多种 `JsonSerializerOptions`，支持驼峰命名、中文字符编码、忽略大小写等多种配置。
- **JSON 扩展函数**：提供了用于序列化和反序列化的扩展方法，支持不同的压缩级别和自定义配置选项。

## 主要类和文件
- **CommonSerializerOptions.cs**：定义了各种 `JsonSerializerOptions` 的静态实例，方便在项目中复用。
- **JsonExtends.cs**：包含 JSON 序列化和反序列化的扩展方法，支持不同配置选项和压缩级别。
- **Converters/**：包含多个自定义的 JSON 转换器，如 `DateTime2StringConverter` 和 `Long2StringConverter`。

## 自定义转换器 (Converters)
### DateTime2StringConverter
将 `DateTime` 类型转换为指定格式的字符串。

#### 使用示例
```csharp
using System.Text.Json;
using Asgard.Extends.Json.Converters;

var options = new JsonSerializerOptions();
options.Converters.Add(new DateTime2StringConverter("yyyy-MM-dd HH:mm"));

var dateTime = DateTime.Now;
string jsonString = JsonSerializer.Serialize(dateTime, options);
```

### Long2StringConverter
将 `long` 类型转换为字符串。

#### 使用示例
```csharp
using System.Text.Json;
using Asgard.Extends.Json.Converters;

var options = new JsonSerializerOptions();
options.Converters.Add(new Long2StringConverter());

long number = 1234567890;
string jsonString = JsonSerializer.Serialize(number, options);
```

## 使用示例
以下是一个简单的使用示例，展示如何使用 `JsonExtends` 类进行 JSON 序列化和反序列化：

```csharp
using Asgard.Extends.Json;

var myObject = new MyClass
{
    Property1 = "Value1",
    Property2 = 123
};

// 序列化为 JSON 字节数组
byte[] jsonBytes = myObject.GetBytes();

// 反序列化为对象
MyClass deserializedObject = jsonBytes.GetObject<MyClass>();
```

## 依赖项
- **Asgard.Tools**：提供了一些辅助工具和方法，如 `BrotliUTF8` 压缩工具。

## 版权信息
本项目遵循 MIT 许可协议。详细信息请参阅 [LICENSE](LICENSE) 文件。
