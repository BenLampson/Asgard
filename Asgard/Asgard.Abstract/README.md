# Asgard.Abstract

Asgard.Abstract 是 Asgard 框架的核心抽象层，提供了认证、缓存、数据库、日志、消息队列等基础组件的抽象接口定义。

## 项目结构

```
Asgard.Abstract/
├── Auth/                    # 认证相关抽象
│   └── AbsAuthManager.cs    # 认证管理器抽象基类
├── Cache/                   # 缓存相关抽象
│   ├── AbsCache.cs         # 缓存管理器抽象基类
│   └── CacheItemSettings.cs # 缓存项配置
├── DataBase/               # 数据库相关抽象
│   └── AbsDataBaseManager.cs # 数据库管理器抽象基类
├── Logger/                 # 日志相关抽象
│   ├── AbsLogger.cs        # 日志记录器抽象基类
│   └── AbsLoggerProvider.cs # 日志提供器抽象基类
├── MQ/                     # 消息队列相关抽象
│   ├── AbsMQManager.cs     # 消息队列管理器抽象基类
│   ├── MQHostInfo.cs       # 消息队列主机信息
│   └── MQItemOptions.cs    # 消息项配置选项
├── Models/                 # 模型定义
│   ├── AsgardConfig/       # 配置模型
│   └── AsgardUserInfo/     # 用户信息模型
└── AsgardContext.cs        # 核心上下文类
```

## 核心功能

### 认证管理 (Auth)
- **AbsAuthManager**: JWT Token 的创建、解析和刷新功能的抽象接口
  - 支持 Token 创建与验证
  - 支持用户信息解析
  - 支持刷新 Token 生成

### 缓存管理 (Cache)
- **AbsCache**: 统一缓存接口抽象
- **CacheItemSettings**: 缓存项配置管理

### 数据库管理 (DataBase)
- **AbsDataBaseManager**: 数据库操作的抽象接口

### 日志管理 (Logger)
- **AbsLogger**: 日志记录器抽象接口
- **AbsLoggerProvider**: 日志提供器抽象接口

### 消息队列 (MQ)
- **AbsMQManager**: 消息队列管理抽象接口
- **MQHostInfo**: 消息队列主机配置
- **MQItemOptions**: 消息项配置选项

### 配置管理
提供完整的配置模型，包括：
- 认证配置 (AuthConfig)
- 配置中心 (ConfigCenterConfig)
- 数据库配置 (DefaultDBConfig)
- 分布式ID生成器 (IDWorker)
- 消息队列配置 (MQConfig)
- Redis配置 (RedisConfig)
- 静态文件配置 (StaticFileConfig)
- Web API配置 (WebApiConfig)
- 日志配置 (LogConfig)

## 使用说明

本项目作为 Asgard 框架的抽象层，主要供具体实现项目引用。各模块的具体实现应继承相应的抽象类并实现其抽象方法。

### 开发规范
- 所有抽象类必须使用 `Abs` 前缀命名
- 提供完整的 XML 注释文档
- 遵循单一职责原则
- 接口设计应具有良好的扩展性

## 技术栈
- .NET Standard
- C# 10.0+
- JWT 认证
- 支持多种缓存实现
- 支持多种消息队列

## 许可证
MIT License