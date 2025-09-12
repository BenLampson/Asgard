using System.Net;
using System.Net.Sockets;

using Asgard.Abstract.Logger;

namespace Asgard.Abstract.Communication.Tcp
{
    /// <summary>
    /// TCP通信服务抽象基类
    /// </summary>    
    public class TcpServer<T> : IDisposable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; protected set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 是否已经销毁
        /// </summary>
        protected bool _disposed;

        /// <summary>
        /// 日志对象
        /// </summary>
        protected readonly AbsLogger _logger;
        /// <summary>
        /// 日志提供器
        /// </summary>
        protected readonly AbsLoggerProvider _loggerProvider;

        /// <summary>
        /// 当获取到数据时事件
        /// </summary>
        public event Action<TcpClient<T>>? GotClient;

        /// <summary>
        /// 是否再运行中
        /// </summary>
        protected bool _started;

        /// <summary>
        /// 原始套接字对象
        /// </summary>
        protected readonly Socket _socket;

        /// <summary>
        /// 绑定地址
        /// </summary>
        protected readonly IPEndPoint _ipendPoint;

        /// <summary>
        /// 取消token
        /// </summary>
        protected readonly CancellationTokenSource _cancellToken = new();

        private readonly Func<AbsTcpPackage<T>> _packageCreator;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iPEndPoint">目标绑定地址</param>
        /// <param name="logger">日志</param>
        /// <param name="name">名称</param>
        /// <param name="packageCreator">对应包构造器,入参代表着此时是主动收到的包</param>
        public TcpServer(IPEndPoint iPEndPoint, AbsLoggerProvider logger, string name, Func<AbsTcpPackage<T>> packageCreator)
        {
            _packageCreator = packageCreator;
            _loggerProvider = logger;
            Name = name;
            _ipendPoint = iPEndPoint;
            _logger = logger.CreateLogger(nameof(TcpServer<T>));
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }


        /// <summary>
        /// 析构函数
        /// </summary>
        ~TcpServer()
        {
            Dispose(false);
        }

        /// <summary>
        /// 销毁模型
        /// </summary>
        /// <param name="flag">是否是用户主动调用</param>
        protected virtual void Dispose(bool flag)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;


            try
            {
                _cancellToken.Cancel();
                _cancellToken.Dispose();
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                _socket.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error("AbsCommServer dispose error.", exception: ex);
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
        /// 开始服务
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                if (_started)
                {
                    return;
                }
                _socket.Bind(_ipendPoint);
                _started = true;
                _socket.Listen(0);
                _ = Task.Factory.StartNew(async () =>
                {
                    while (!_cancellToken.Token.IsCancellationRequested)
                    {
                        try
                        {
                            var client = await _socket.AcceptAsync(_cancellToken.Token);
                            _ = Task.Run(() =>
                            {
                                GotClient?.Invoke(GetClient($"{Name}_client_{Guid.NewGuid():N}", client));
                            });
                        }
                        catch (SocketException ex)
                        {
                            _logger.Critical($"Got socket exception, server {_ipendPoint} shutdown.", exception: ex);
                            break;
                        }
                        catch (ObjectDisposedException ex)
                        {
                            _logger.Warning($"Maybe you dispose this instance so fast than socket sutdown {_socket.RemoteEndPoint} closed.", exception: ex);
                            break;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Got AcceptAsync exception.", exception: ex);
                            continue;
                        }
                    }
                }, _cancellToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _cancellToken.Cancel();
            _started = false;
        }


        /// <summary>
        /// 获取一个当前服务的客户端
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="localInfo">本地地址信息</param>
        /// <returns></returns>
        public TcpClient<T> GetClient(string name, IPEndPoint? localInfo = null)
        {
            return new TcpClient<T>(_ipendPoint, _loggerProvider, name, localInfo, _packageCreator);
        }


        /// <summary>
        /// 获取一个当前服务的客户端
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="socket">对应通信句柄</param>
        /// <returns></returns>
        public TcpClient<T> GetClient(string name, Socket socket)
        {
            return new TcpClient<T>(_ipendPoint, _loggerProvider, name, socket, _packageCreator);
        }
    }
}
