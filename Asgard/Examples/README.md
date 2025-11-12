# Asgard.AspNetCore.Full 示例项目说明

本项目为 Asgard 框架下的 ASP.NET Core 完整示例，演示了如何集成数据库、缓存、认证、日志等模块，并实现一个简单的 API 控制器。

## 目录结构

- `Program.cs`：项目启动入口，负责配置各模块并启动主机。
- `Bifrost.cs`：主机插件入口，扩展系统生命周期相关事件。
- `Controllers/HelloWorldController.cs`：API 控制器示例，演示接口定义与认证。

## 使用步骤

### 1. 安装依赖

确保已安装 .NET 6 或更高版本。

```bash
dotnet restore
```

### 2. 构建项目

```bash
dotnet build
```

### 3. 运行示例

```bash
dotnet run --project Asgard.AspNetCore.Full/Asgard.AspNetCore.Full.csproj
```

### 4. 访问接口

启动后可访问 `http://localhost:{端口}/Asgard/HelloWorld`，接口需要认证，具体认证参数见 `Program.cs` 的 `AuthConfig` 配置。

## 主要模块说明

- **数据库管理**：默认使用 SQLite，配置见 `Program.cs` 的 `DefaultDB` 部分。
- **认证模块**：自动生成 AES 密钥与 JWT Key，保证接口安全。
- **日志模块**：支持控制台日志输出，便于调试与追踪。
- **插件机制**：通过 `Bifrost.cs` 扩展主机功能，支持系统启动、服务初始化等生命周期事件。
- **API 控制器**：`HelloWorldController` 演示了接口定义、认证注解与异常处理。

## 代码流程简述

1. 初始化配置（数据库、认证、日志等）。
2. 构建主机并加载各模块。
3. 加载插件并启动 ASP.NET Core 服务。
4. 通过控制器对外提供 API 服务。

## 注意事项

- 示例接口默认抛出异常，仅供演示如何定义接口与认证。
- 可根据实际需求扩展控制器与业务逻辑。
