# Asgard.Tools

Asgard.Tools 是一个包含多种实用工具的 .NET 库，旨在简化开发过程中的常见任务，如加密、压缩和枚举处理。

## 项目结构

```
Asgard.Tools/
├── Asgard.Tools.csproj
├── AuthKVToolsMethod.cs
├── BaseEncryptionTools.cs
├── BrotliUTF8.cs
├── EnumExtendsMethods.cs
└── Wildcard.cs
```

## 核心功能

### 加密工具 (AuthKVToolsMethod.cs)
- **CreateNewAesKeyAndVi**: 创建新的 AES 加密密钥和 IV。
- **CreateNewHMACSHA256Key**: 创建新的 HMACSHA256 密钥。

### 基础加密工具 (BaseEncryptionTools.cs)
- **SetKeyAndIV**: 设置 AES 密钥和 IV。
- **Md5**: 使用 MD5 对字符串进行加密。
- **EncryptStringToString_Aes**: 使用 AES 加密字符串。
- **DecryptStringFromStr_Aes**: 使用 AES 解密字符串。

### Brotli 压缩工具 (BrotliUTF8.cs)
- **Compress**: 压缩字符串或字节数组。
- **CompressHightLevel**: 使用高压缩级别压缩字符串或字节数组。
- **Decompress**: 解压缩字节数组。
- **GetString**: 解压缩字节数组并返回字符串。

### 枚举扩展方法 (EnumExtendsMethods.cs)
- **GetEnumDescriptionOriginal**: 获取枚举成员的描述。

### 通配符匹配工具 (Wildcard.cs)
- **IsMatch**: 判断文本是否匹配给定的通配符模式。

## 使用说明

### 安装

可以通过 NuGet 安装 Asgard.Tools：

```bash
dotnet add package Asgard.Tools
```

### 示例

#### AES 加密和解密

```csharp
using Asgard.Tools;

var (key, iv) = AuthKVToolsMethod.CreateNewAesKeyAndVi();
BaseEncryptionTools.SetKeyAndIV(key, iv);

string originalText = "Hello, Asgard!";
string encryptedText = BaseEncryptionTools.EncryptStringToString_Aes(originalText);
string decryptedText = BaseEncryptionTools.DecryptStringFromStr_Aes(encryptedText);

Console.WriteLine($"Original: {originalText}");
Console.WriteLine($"Encrypted: {encryptedText}");
Console.WriteLine($"Decrypted: {decryptedText}");
```

#### Brotli 压缩和解压缩

```csharp
using Asgard.Tools;

string text = "This is a sample text to compress.";
byte[] compressed = BrotliUTF8.Compress(text);
string decompressed = BrotliUTF8.GetString(compressed);

Console.WriteLine($"Original: {text}");
Console.WriteLine($"Compressed: {Convert.ToBase64String(compressed)}");
Console.WriteLine($"Decompressed: {decompressed}");
```

#### 枚举扩展方法

```csharp
using Asgard.Tools;
using System.ComponentModel;

public enum SampleEnum
{
    [Description("First Option")]
    FirstOption,

    [Description("Second Option")]
    SecondOption,

    ThirdOption
}

SampleEnum value = SampleEnum.FirstOption;
string description = value.GetEnumDescriptionOriginal();

Console.WriteLine(description); // 输出: First Option
```

#### 通配符匹配

```csharp
using Asgard.Tools;

bool match1 = Wildcard.IsMatch("hello-world", "hello-*");
bool match2 = Wildcard.IsMatch("hello-world", "hi-*");

Console.WriteLine(match1); // 输出: True
Console.WriteLine(match2); // 输出: False
```

## 许可证

MIT License