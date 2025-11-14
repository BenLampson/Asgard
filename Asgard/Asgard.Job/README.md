# Asgard.Job

## 项目简介

Asgard.Job 是 Asgard 框架的任务调度与后台作业执行模块，支持定时任务、延迟任务、分布式调度与任务持久化，适用于微服务和分布式场景下的异步作业管理。兼容 .NET 8，并可与 Asgard 框架其他模块集成。

## 主要类与接口说明

- [`JobManager`](Asgard.Job/JobManager.cs:10)：任务管理器，负责任务的注册、启动、停止及生命周期管理。
- [`JobBase`](Asgard.Job/JobBase.cs:11)：任务抽象基类，所有自定义任务需继承该类，实现 `Start` 和 `Stop` 方法。
- [`JobInfoItem`](Asgard.Job/JobInfoItem.cs:8)：任务元数据与实例管理，内部用于存储任务构造器、类型、定时参数等信息。
- [`JobTypeEnum`](Asgard.Job/JobTypeEnum.cs:6)：任务类型枚举，支持 Singleton（单例）与 Scoped（每次执行新建）。
- [`JobTimerTypeEnum`](Asgard.Job/JobTimerTypeEnum.cs:6)：定时类型枚举，支持 Independent（独立计时）与 Dependent（依赖上次完成）。

## 快速开始

1. 在主项目中引用 Asgard.Job：

2. 创建自定义任务：

   ```csharp
   using Asgard.Job;
   using Asgard.Abstract;

   public class MyJob : JobBase
   {
       public MyJob(AbsLogger logger) : base(logger) { }

       public override async Task Start(AsgardContext context)
       {
           // 任务逻辑
       }

       public override async Task Stop(AsgardContext context)
       {
           // 停止逻辑
       }
   }
   ```

3. 注册并启动任务：

   ```csharp
   var jobManager = new JobManager(loggerProvider);
   jobManager.PushNewJobInfo(typeof(MyJob));
   jobManager.Start();
   ```

## 依赖说明

- 依赖 [`Asgard.Abstract`](../Asgard.Abstract/Asgard.Abstract.csproj) 模块。
- 需 .NET 8 及以上环境。

## 维护者

- 维护者：Asgard Framework 团队
- 联系方式：暂无（如需协助请通过主项目渠道反馈）
