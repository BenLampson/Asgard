using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

using Asgard.Core.ContextModules.Logger.AbsInfos;

namespace Asgard.Core.ContextModules.Comm.Tcp
{
    /// <summary>
    /// 附带具体类型的TCP通信客户端
    /// </summary>
    /// <typeparam name="T">你的通信数据类型</typeparam>
    public class TcpClient<T> : IDisposable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string ID { get; protected set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 是否已经销毁
        /// </summary>
        public bool Disposed;
        /// <summary>
        /// 原始套接字对象
        /// </summary>
        protected readonly Socket _socket;
        /// <summary>
        /// 目标服务器
        /// </summary>
        protected readonly IPEndPoint _target;

        /// <summary>
        /// 日志对象
        /// </summary>
        protected readonly AbsLogger _logger;

        /// <summary>
        /// 我创建的包池,用于处理等待包
        /// </summary>
        private readonly ConcurrentDictionary<string, AbsTcpPackage<T>> _createdPackagePool = new();

        /// <summary>
        /// 取消token
        /// </summary>
        protected readonly CancellationTokenSource _cancellToken = new();

        /// <summary>
        /// 当前服务的缓冲池
        /// </summary>
        protected readonly MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;

        /// <summary>
        /// 远程地址
        /// </summary>
        public EndPoint? RemotePoint { get => _socket.RemoteEndPoint; }

        /// <summary>
        /// 远程地址
        /// </summary>
        public EndPoint? LocalPoint { get => _socket.LocalEndPoint; }
        /// <summary>
        /// 销毁中事件
        /// </summary>
        public event Action<string>? Disposing;

        /// <summary>
        /// 当获取到数据时事件
        /// </summary>
        public event Action<AbsTcpPackage<T>>? GetPackage;
        /// <summary>
        /// 包构造器
        /// </summary>

        private readonly Func<AbsTcpPackage<T>> _packageCreator;

        /// <summary>
        /// 断开链接事件
        /// </summary>
        public event Action<TcpClient<T>>? OnDisConnected;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="targetIp">目标IP</param>
        /// <param name="logger">日志</param>
        /// <param name="name">名称</param>
        /// <param name="localInfo">本地地址信息</param>
        /// <param name="packageCreator">包创建器</param>
        public TcpClient(IPEndPoint targetIp, AbsLoggerProvider logger, string name, IPEndPoint? localInfo, Func<AbsTcpPackage<T>> packageCreator)
        {
            _target = targetIp;
            _logger = logger.CreateLogger(nameof(TcpClient<T>));
            ID = name;
            _packageCreator = packageCreator;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (localInfo != null)
            {
                _socket.Bind(localInfo);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="targetIp">目标IP</param>
        /// <param name="logger">日志</param>
        /// <param name="name">名称</param>
        /// <param name="socket">对应通信对象</param>
        /// <param name="createPackage">包创建器</param>
        public TcpClient(
            IPEndPoint targetIp,
            AbsLoggerProvider logger,
            string name,
            Socket socket,
            Func<AbsTcpPackage<T>> createPackage)
        {
            _packageCreator = createPackage;
            _target = targetIp;
            _logger = logger.CreateLogger(nameof(TcpClient<T>));
            ID = name;
            _socket = socket;
            HandleClient();
        }


        /// <summary>
        /// 析构函数
        /// </summary>
        ~TcpClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// 销毁模型
        /// </summary>
        /// <param name="flag">是否是用户主动调用</param>
        protected virtual void Dispose(bool flag)
        {
            if (Disposed)
            {
                return;
            }
            Disposing?.Invoke(ID);
            Disposed = true;
            try
            {
                _cancellToken.Cancel();
                _cancellToken.Dispose();
                _memoryPool.Dispose();
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                _socket.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error("AbsCommTcpClient dispose error.", exception: ex);
            }
        }

        /// <summary>
        /// 销毁模型
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (_socket.Connected)
            {
                return true;
            }
            _socket.Connect(_target);
            HandleClient();
            return _socket.Connected;
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _cancellToken.Cancel();
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            return _socket.Connected;
        }


        /// <summary>
        /// 启动服务
        /// </summary>
        private void HandleClient()
        {
            var headSize =
            _ = Task.Factory.StartNew(async () =>
            {
                Memory<byte> headBuffer = Memory<byte>.Empty;
                Memory<byte> flagBuffer = Memory<byte>.Empty;
                var breakOut = false;
                var thisLoopReceived = 0;
                while (!_cancellToken.IsCancellationRequested && !_cancellToken.Token.IsCancellationRequested)
                {
                    try
                    {
                        var package = _packageCreator();
                        if (package._flagSize != 0 && flagBuffer.Equals(Memory<byte>.Empty))
                        {
                            flagBuffer = new byte[package._flagSize];
                        }
                        if (headBuffer.Equals(Memory<byte>.Empty))
                        {
                            headBuffer = new byte[package._headSize];
                        }
                        package.TcpClient = this;
                        thisLoopReceived = 0;
                        Memory<byte> headerID = Memory<byte>.Empty;
                        int bodyLength = 0;
                        while (!_cancellToken.IsCancellationRequested && !_cancellToken.Token.IsCancellationRequested)
                        {
                            if (package._flagSize != 0)
                            {
                                var flag = await _socket.ReceiveAsync(flagBuffer, SocketFlags.None, _cancellToken.Token);
                                if (flag == 0)
                                {
                                    breakOut = true;
                                    break;
                                }
                                if (!package.CheckFlag(flagBuffer))
                                {
                                    package.Dispose();
                                    continue;
                                }
                            }

                            var res = await _socket.ReceiveAsync(headBuffer, SocketFlags.None, _cancellToken.Token);

                            if (res == 0)
                            {
                                breakOut = true;
                                break;
                            }
                            if (!package.CheckHeader(flagBuffer, headBuffer, out bodyLength, out headerID))
                            {
                                package.Dispose();
                                continue;
                            }
                            break;

                        }

                        using var bodyBufferOwner = _memoryPool.Rent(bodyLength);
                        var bodyBuffer = bodyBufferOwner.Memory[..bodyLength];
                        while (thisLoopReceived < bodyLength)
                        {
                            var tempRecCount = await _socket.ReceiveAsync(bodyBuffer[thisLoopReceived..], SocketFlags.None, _cancellToken.Token);

                            if (tempRecCount == 0)
                            {
                                breakOut = true;
                                break;
                            }
                            thisLoopReceived += tempRecCount;
                        }
                        if (breakOut)
                        {
                            break;
                        }
                        var dataInfo = package.HandleData(flagBuffer, headBuffer, bodyBuffer);
                        if (_createdPackagePool.TryGetValue(BitConverter.ToString(headerID.ToArray()), out var oldPackage))
                        {
                            oldPackage.In = dataInfo;
                            package.Dispose();
                        }
                        else
                        {
                            package.In = dataInfo;
                            _ = Task.Run(() =>
                            {
                                GetPackage?.Invoke(package);
                                if (!package.KeepAlive)
                                {
                                    package.Dispose();
                                }
                            });

                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (SocketException ex)
                    {
                        _logger.Critical($"Got socket exception, _socket {_socket.RemoteEndPoint} closed.", exception: ex);
                        break;
                    }
                    catch (ObjectDisposedException ex)
                    {
                        _logger.Warning($"Maybe you dispose this instance so fast than socket sutdown {_socket.RemoteEndPoint} closed.", exception: ex);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Got ReceiveAsync data exception.", exception: ex);
                        continue;
                    }

                }
                if (!Disposed)
                {
                    try
                    {
                        _logger.Information($"Client {_socket.RemoteEndPoint} closed.");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Close _socket {_socket.RemoteEndPoint} got exception.", exception: ex);
                    }
                }
                OnDisConnected?.Invoke(this);
            }, _cancellToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }


        /// <summary>
        /// 发送数据
        /// </summary> 
        /// <param name="data">数据</param>
        /// <returns></returns>
        internal bool Send(Memory<byte> data)
        {
            return _socket.Send(data.Span) == data.Length;
        }


        /// <summary>
        /// 从该客户端创建一个包
        /// </summary>
        /// <returns></returns>
        public AbsTcpPackage<T> CreatePackage()
        {
            var package = _packageCreator();
            package.TcpClient = this;
            package.KeepAlive = true;
            package.OnDisposing += Package_OnDisposing;
            _ = _createdPackagePool.TryAdd(BitConverter.ToString(package.ID.ToArray()), package);
            return package;
        }

        /// <summary>
        /// 包销毁订阅
        /// </summary>
        /// <param name="id">包ID</param>
        private void Package_OnDisposing(Memory<byte> id)
        {
            if (_createdPackagePool.TryRemove(BitConverter.ToString(id.ToArray()), out var package))
            {
                package.OnDisposing -= Package_OnDisposing;
            }

        }
    }
}
