# Asgard.Abstract

Asgard.Abstract 是 Asgard 框架的核心抽象层，定义了所有关键组件的接口契约。它是框架可扩展性的基石，任何具体实现都必须遵循这些约定。

## 项目结构

```
Asgard.Abstract/
├── Auth/                    # 认证相关抽象
│   └── AbsAuthManager.cs    # JWT Token 管理的抽象基类
├── Cache/                   # 缓存相关抽象
│   ├── AbsCache.cs         # 统一缓存操作的抽象基类
│   └── CacheItemSettings.cs # 缓存项过期策略等配置
├── DataBase/               # 数据库相关抽象
│   └── AbsDataBaseManager.cs # 数据库会话与事务管理的抽象基类
├── Logger/                 # 日志相关抽象
│   ├── AbsLogger.cs        # 日志记录器的抽象基类
│   └── AbsLoggerProvider.cs # 日志提供器（如控制台、文件、数据库）的抽象基类
├── MQ/                     # 消息队列相关抽象
│   ├── AbsMQManager.cs     # 消息发布/订阅的抽象基类
│   ├── MQHostInfo.cs       # 消息队列主机连接信息
│   └── MQItemOptions.cs    # 消息持久化、重试等选项配置
├── Models/                 # 核心模型定义
│   ├── AsgardConfig/       # 包含所有系统模块的配置模型 (如 ConfigCenterConfig, WebApiConfig)
│   └── AsgardUserInfo/     # 用户认证信息模型
└── AsgardContext.cs        # 封装了当前运行时所需的所有服务实例 (DB, Logger, Cache 等) 的核心上下文
```

## 抽象接口详解

### `AbsBifrost` - 系统入口点
所有宿主和插件的入口都必须继承此类。它定义了系统生命周期的关键方法：
- `OnSystemStarted(AsgardContext context)`: 系统完全启动后调用。
- `SystemTryShutDown()`: 系统即将关闭时调用。

### `AsgardContext` - 运行时上下文
这是一个至关重要的对象，它在系统启动时被创建，并作为参数传递给 `OnSystemStarted` 方法。它包含了对数据库、日志、缓存等服务的引用，是业务逻辑访问基础设施的统一入口。

## 开发规范

- **命名**: 所有抽象类和接口必须使用 `Abs` 前缀。
- **职责**: 遵循单一职责原则，每个抽象只负责一个明确的功能领域。
- **扩展性**: 接口设计应考虑未来可能的扩展，避免频繁修改。
- **文档**: 提供完整的 XML 文档注释。

## 许可证

MIT License