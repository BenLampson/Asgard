# Asgard - 尚书省

Asgard（尚书省）是一个高度模块化、可扩展的 .NET 后端应用框架。它通过**中心化配置**和**插件化架构**，旨在简化分布式系统的开发、部署和维护。

> **核心理念**: “一次配置，处处运行；按需加载，动态扩展。”

![Asgard Architecture](logo.png)

## 🌟 核心特性

- **中心化配置管理**: 所有服务节点的配置由统一的配置中心管理，支持动态更新。
- **强大的插件系统**: 业务功能以插件形式 (.dll) 开发和部署，实现热插拔和独立演进。
- **自动生成前端代码**: 分析 C# API 控制器，一键生成 TypeScript 客户端代码和类型定义。
- **开箱即用的基础设施**: 内置对 FreeSql (ORM)、RabbitMQ、Redis、JWT 认证等的支持。
- **统一的日志与监控**: 提供结构化的日志记录和系统状态监控能力。

## 📦 项目结构

```
Asgard/
├── Asgard.Abstract/            # 核心抽象层 (接口定义)
├── SystemModules/              # 系统模块实现 (FreeSql, Redis, RabbitMQ 等)
├── Hosts/                      # 应用宿主模板 (ASP.NET Core + FreeSql)
├── Examples/                   # 使用示例
└── Asgard.Tools/               # 工具类库
```

## 🚀 快速开始

### 方式一：通过配置中心启动 (推荐)

1.  **创建宿主项目**
    复制 `Hosts/Asgard.Hosts.AspNetCore.FreeSql` 目录作为你的新项目起点。

2.  **实现业务入口**
    创建一个继承自 `AbsAspNetCoreHostBifrost` 的类，例如 `MyAppBifrost.cs`:
    ```csharp
    public class MyAppBifrost : AbsAspNetCoreHostBifrost
    {
        public MyAppBifrost(AbsDataBaseManager db, AbsLoggerProvider logger) 
            : base(db, logger) { }

        public override void OnSystemStarted(AsgardContext context)
        {
            // 系统启动后执行的初始化逻辑
        }

        public override void OnBuildWebApp(IApplicationBuilder builder) { }
        public override void OnServiceInit(IServiceCollection service) { }
        public override void SystemTryShutDown() { }
    }
    ```

3.  **启动应用**
    通过命令行指定配置中心地址来启动你的应用：
    ```bash
    dotnet YourApp.dll pn:YourNodeName csa:192.168.1.100 csp:12341
    ```
    - `pn`: 在配置中心注册的节点名称。
    - `csa`: 配置中心的 IP 地址。
    - `csp`: 配置中心的端口号。

### 方式二：通过本地配置文件启动

1.  在你的宿主项目根目录下创建 `appsettings.json` 文件，并填入你的节点配置。
2.  直接运行应用即可，框架会自动读取该文件进行初始化。

## 🧩 插件开发

1.  创建一个新的类库项目。
2.  引用 `Asgard.Abstract`。
3.  实现你的业务逻辑，并创建一个继承自 `AbsBifrost` 的入口类。
4.  将编译后的 `.dll` 文件放入宿主项目的 `Plugins` 文件夹。
5.  在配置中心为你的节点添加此插件的配置。

## 📖 子模块文档

- [`Asgard.Abstract`](Asgard.Abstract/README.md): 核心抽象层详细说明。
- [`SystemModules`](SystemModules/): 各系统模块（数据库、缓存、MQ等）的实现细节。

## 📜 许可证

MIT License