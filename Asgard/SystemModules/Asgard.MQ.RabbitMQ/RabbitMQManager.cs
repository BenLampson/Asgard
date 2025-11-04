using System.Collections.Concurrent;
using System.Text;

using Asgard.Abstract.Logger;
using Asgard.Abstract.Models.AsgardConfig.ChildConfigModel;
using Asgard.Abstract.MQ;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Asgard.MQ.RabbitMQ
{
    /// <summary>
    /// RabbitMQ manager
    /// </summary>
    public class RabbitMQManager : AbsMQManager
    {


        /// <summary>
        /// Channel pool
        /// </summary>
        private readonly ConcurrentDictionary<string, IChannel> _channelPool = new();
        private readonly string _userName;
        private readonly string _pwd;
        private readonly string _host;
        private readonly string _defaultHost;

        /// <summary>
        /// 
        /// </summary> 
        public RabbitMQManager(AbsLoggerProvider logger, MQConfig mQConfig) : base(logger.CreateLogger<RabbitMQManager>())
        {
            _userName = mQConfig.UserName;
            _pwd = mQConfig.Password;
            _host = mQConfig.Server;
            _defaultHost = $"{_host}_SendMeessageToChannel";
        }


        /// <summary>
        /// Connect to a service
        /// </summary>
        private IConnection GetConnect(string host, string userName, string password, string virtualHostName)
        {
            Uri uri = new(host);
            ConnectionFactory connectionFactory = new()
            {
                UserName = userName,
                Password = password,
                VirtualHost = virtualHostName,
                Endpoint = new AmqpTcpEndpoint(uri),
                AutomaticRecoveryEnabled = true,
            };

            return connectionFactory.CreateConnectionAsync().Result;
        }

        /// <summary>
        /// Send a message to local MQ service
        /// </summary>
        /// <param name="options">Message parameters</param>
        /// <param name="message">Message content</param>
        /// <param name="hostInfo">Host information, use container configuration if null</param>
        /// <returns></returns>
        public override bool SendMessageTo(SendMQItemOptions options, string message, MQHostInfo? hostInfo = null)
        {
            try
            {
                var channelKey = _defaultHost;
                if (hostInfo is not null)
                {
                    channelKey = $"{hostInfo.Host}_SendMeessageToVirtualHost_{options.VirtualHostName}";
                }
                else
                {
                    channelKey = $"{channelKey}_SendMeessageToVirtualHost_{options.VirtualHostName}";
                }
                var channel = _channelPool.GetOrAdd(channelKey, key =>
                {
                    var host = _host;
                    var userName = _userName;
                    var pwd = _pwd;
                    if (hostInfo is not null)
                    {
                        host = hostInfo.Host;
                        userName = hostInfo.UserName;
                        pwd = hostInfo.Passwords;
                    }

                    var conn = GetConnect(host, userName, pwd, virtualHostName: options.VirtualHostName);
                    var channel = conn.CreateChannelAsync().Result;
                    return channel;
                });

                var props = new BasicProperties();
                if (options.Expiration != 0)
                {
                    props.Expiration = options.Expiration.ToString();
                }
                props.DeliveryMode = (DeliveryModes)options.DeliveryMode; ;
                ValueTask valueTask = channel.BasicPublishAsync(options.ExchangeName, options.RoutingKey, options.Mandatory, props, Encoding.UTF8.GetBytes(message));
                valueTask.AsTask().Wait();
            }
            catch (Exception ex)
            {
                Logger.Error($"Send message to server failed.", exception: ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Send a message to local MQ service
        /// </summary>
        /// <param name="options">Message parameters</param>
        /// <param name="message">Message content</param>
        /// <param name="hostInfo">Host information, use container configuration if null</param>
        /// <returns></returns>
        public override async Task<bool> SendMessageToAsync(SendMQItemOptions options, string message, MQHostInfo? hostInfo = null)
        {
            try
            {
                var channelKey = _defaultHost;
                if (hostInfo is not null)
                {
                    channelKey = $"{hostInfo.Host}_SendMeessageToVirtualHost_{options.VirtualHostName}";
                }
                else
                {
                    channelKey = $"{channelKey}_SendMeessageToVirtualHost_{options.VirtualHostName}";
                }
                var channel = _channelPool.GetOrAdd(channelKey, key =>
                {
                    var host = _host;
                    var userName = _userName;
                    var pwd = _pwd;
                    if (hostInfo is not null)
                    {
                        host = hostInfo.Host;
                        userName = hostInfo.UserName;
                        pwd = hostInfo.Passwords;
                    }

                    var conn = GetConnect(host, userName, pwd, virtualHostName: options.VirtualHostName);
                    var channel = conn.CreateChannelAsync().Result;
                    return channel;
                });

                var props = new BasicProperties();
                if (options.Expiration != 0)
                {
                    props.Expiration = options.Expiration.ToString();
                }
                props.DeliveryMode = (DeliveryModes)options.DeliveryMode; ;
                await channel.BasicPublishAsync(options.ExchangeName, options.RoutingKey, options.Mandatory, props, Encoding.UTF8.GetBytes(message));

            }
            catch (Exception ex)
            {
                Logger.Error($"Send message to server failed.", exception: ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Close a channel
        /// </summary>
        /// <param name="channelToken">Channel key</param>
        public override void CloseConnection(string channelToken)
        {
            if (_channelPool.TryRemove(channelToken, out var channel))
            {
                channel.CloseAsync().Wait();
                channel.Dispose();
            }
        }


        /// <summary>
        /// Close a channel
        /// </summary>
        /// <param name="channelToken">Channel key</param>
        public override async Task CloseConnectionAsync(string channelToken)
        {
            if (_channelPool.TryRemove(channelToken, out var channel))
            {
                await channel.CloseAsync();
                channel.Dispose();
            }
        }

        /// <summary>
        /// Get message from local MQ
        /// </summary>
        /// <param name="virtualHost">Virtual host name</param>
        /// <param name="queueName">Queue name</param>
        /// <param name="callBackMethod">Callback function</param>
        /// <param name="hostInfo">Host information settings, use container configuration if not set</param>
        public override string? GotMessageFrom(string virtualHost, string queueName, Func<string, bool> callBackMethod, MQHostInfo? hostInfo = null)
        {
            var channelToken = Guid.NewGuid().ToString("N");
            var host = _host;
            var userName = _userName;
            var pwd = _pwd;
            if (hostInfo is not null)
            {
                host = hostInfo.Host;
                userName = hostInfo.UserName;
                pwd = hostInfo.Passwords;
            }

            var conn = GetConnect(host, userName, pwd, virtualHostName: virtualHost);


            var channel = conn.CreateChannelAsync().Result;

            try
            {
                var consumer = new AsyncEventingBasicConsumer(channel);//  new EventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, e) =>
                {
                    var message = Encoding.UTF8.GetString(e.Body.Span);

                    await CallbackInvokerAsync(callBackMethod, channel, e.DeliveryTag, message);
                };
                _ = channel.BasicConsumeAsync(queueName, false, channelToken, true, false, null, consumer);
                if (!_channelPool.TryAdd(channelToken, channel))
                {
                    throw new Exception("Try add to pool failed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Get message from server failed.", exception: ex);
                channel.CloseAsync().Wait();
                channel.Dispose();
                return null;
            }
            return channelToken;
        }



        /// <summary>
        /// Get message from local MQ
        /// </summary>
        /// <param name="virtualHost">Virtual host name</param>
        /// <param name="queueName">Queue name</param>
        /// <param name="callBackMethod">Callback function</param>
        /// <param name="hostInfo">Host information settings, use container configuration if not set</param>
        public override async Task<string?> GotMessageFromAsync(string virtualHost, string queueName, Func<string, Task<bool>> callBackMethod, MQHostInfo? hostInfo = null)
        {
            var channelToken = Guid.NewGuid().ToString("N");
            var host = _host;
            var userName = _userName;
            var pwd = _pwd;
            if (hostInfo is not null)
            {
                host = hostInfo.Host;
                userName = hostInfo.UserName;
                pwd = hostInfo.Passwords;
            }

            var conn = GetConnect(host, userName, pwd, virtualHostName: virtualHost);


            var channel = await conn.CreateChannelAsync();
            try
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, e) =>
                {
                    var message = Encoding.UTF8.GetString(e.Body.Span);

                    await CallbackInvokerAsync(callBackMethod, channel, e.DeliveryTag, message);
                };
                _ = channel.BasicConsumeAsync(queueName, false, channelToken, true, false, null, consumer);
                if (!_channelPool.TryAdd(channelToken, channel))
                {
                    throw new Exception("Try add to pool failed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Get message from server failed.", exception: ex);
                channel.CloseAsync().Wait();
                channel.Dispose();
                return null;
            }
            return channelToken;
        }

        /// <summary>
        /// Invoke specific callback function
        /// </summary>
        /// <param name="callBackMethod">Callback function</param>
        /// <param name="channel">Channel</param>
        /// <param name="tag">Tag</param>
        /// <param name="message">Message body</param>
        private async Task CallbackInvokerAsync(Func<string, Task<bool>> callBackMethod, IChannel channel, ulong tag, string message)
        {
            if (callBackMethod is null)
            {
                return;
            }
            try
            {
                var es = await callBackMethod.Invoke(message);
                await channel.BasicNackAsync(tag, false, !es);
            }
            catch (Exception ex)
            {
                Logger.Error($"Invoke callback failed.", exception: ex);
                await channel.BasicNackAsync(tag, false, true);
            }
        }
        /// <summary>
        /// Invoke specific callback function
        /// </summary>
        /// <param name="callBackMethod">Callback function</param>
        /// <param name="channel">Channel</param>
        /// <param name="tag">Tag</param>
        /// <param name="message">Message body</param>
        private async Task CallbackInvokerAsync(Func<string, bool> callBackMethod, IChannel channel, ulong tag, string message)
        {
            try
            {
                var es = callBackMethod?.Invoke(message);
                await channel.BasicNackAsync(tag, false, es == null || !es.Value);
            }
            catch (Exception ex)
            {
                Logger.Error($"Invoke callback failed.", exception: ex);
                await channel.BasicNackAsync(tag, false, true);
            }
        }

    }
}
