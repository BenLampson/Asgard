# Asgard.Hosts.AspNetCore

## 项目简介
Asgard.Hosts.AspNetCore 是一个高度模块化的 ASP.NET Core 宿主框架，专为分布式系统和插件式架构设计，支持灵活扩展和高性能服务承载。

## 主要特性
- 插件式架构，支持热插拔和动态加载
- 支持多种中间件和服务注册
- 内置跨域、静态文件、Swagger、WebSocket、GRPC等常用功能
- 与 Asgard.Abstract、Asgard.Extends 等模块无缝集成
- 适用于微服务、后台管理、API网关等多种场景

## 快速开始

```csharp
var builder = new YggdrasilBuilder();
// 配置各项服务
var host = builder.BuildAspNetCoreHost();
await host.StartAsync();
```

## NuGet 包信息
- 包名：Asgard.Hosts.AspNetCore
- 支持平台：.NET 8.0 及以上
- 安装方式：
```shell
dotnet add package Asgard.Hosts.AspNetCore
```

## 典型应用场景
- 微服务网关
- 后台管理系统
- 高性能 API 服务
- 插件式业务扩展

## 扩展与定制
支持自定义插件、日志、缓存、认证、数据库等模块，详见接口文档和源码注释。

## 许可证
MIT

---

如需详细文档和示例，请参考源码及相关模块的 README。

```mermaid
flowchart TD
    A[Asgard.Hosts.AspNetCore] --> B[插件管理]
    A --> C[中间件扩展]
    A --> D[服务注册]
    A --> E[静态文件/Swagger/WebSocket/GRPC]