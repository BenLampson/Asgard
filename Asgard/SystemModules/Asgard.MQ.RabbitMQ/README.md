# Asgard.MQ.RabbitMQ

本包用于Asgard框架的RabbitMQ消息队列模块，支持高效的消息分发与处理。

## 使用方法

1. 在你的项目中引用本包。
2. 配置RabbitMQ连接参数（如主机、端口、用户名、密码等）。
3. 使用`RabbitMQManager`类进行消息的发送与接收。

示例代码：

```csharp
var manager = new RabbitMQManager("amqp://user:password@localhost:5672/");
manager.Publish("queueName", "消息内容");
manager.Subscribe("queueName", msg => {
    Console.WriteLine($"收到消息: {msg}");
});
```

更多详细用法请参考源码或相关文档。