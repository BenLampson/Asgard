using System.Buffers;

using Asgard.Abstract.Logger;

namespace Asgard.Abstract.Communication.Tcp
{
    /// <summary>
    /// 抽象的TCP通信包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbsTcpPackage<T> : IDisposable
    {
        /// <summary>
        /// 是否已经销毁
        /// </summary>
        public bool Disposed;
        /// <summary>
        /// 包ID
        /// </summary>
        public Memory<byte> ID { get; protected set; }

        /// <summary>
        /// 头长度 默认20,4长度+16GUID
        /// </summary>
        protected internal int _headSize = 4 + 16;
        /// <summary>
        /// 销毁事件
        /// </summary>
        public event Action<Memory<byte>>? OnDisposing;

        /// <summary>
        /// 当前服务的缓冲池
        /// </summary>
        protected readonly MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;
        /// <summary>
        /// 等待锁
        /// </summary>
        private readonly ManualResetEvent _waitter = new(false);

        /// <summary>
        /// TCP客户端对象
        /// </summary>
        public TcpClient<T>? TcpClient { get; internal set; }

        /// <summary>
        /// 是否保持包存活,如果是true,则需要用户自己考虑生命周期
        /// </summary>
        public bool KeepAlive { get; set; } = false;

        /// <summary>
        /// 标识位长度
        /// </summary>
        protected internal int _flagSize = 2;

        private T? _in;
        /// <summary>
        /// 收到的数据
        /// </summary>
        public T? In
        {
            get => _in;
            internal set
            {
                if (Disposed) return;
                _in = value;
                _ = _waitter.Set();
            }
        }
        /// <summary>   
        /// 发送的数据
        /// </summary>
        public T? Out { get; internal set; }

        /// <summary>
        /// 日志对象
        /// </summary>
        protected readonly AbsLogger? _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志模块</param>
        /// <param name="id">ID</param>
        public AbsTcpPackage(AbsLogger? logger, Memory<byte> id)
        {
            ID = id;
            _logger = logger;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~AbsTcpPackage()
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
            Disposed = true;
            _logger?.Trace($"包被销毁.");
            try
            {
                _ = _waitter.Set();
                OnDisposing?.Invoke(ID);
            }
            catch (Exception ex)
            {
                _logger?.Error("关闭TCP客户端报错", exception: ex);
            }
            try
            {
                _memoryPool.Dispose();
                _waitter.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.Error("销毁TCP客户端报错", exception: ex);
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
        /// 打包数据
        /// </summary>
        /// <returns></returns>
        public Memory<byte> Pack()
        {
            if (Out is null)
            {
                return null;
            }
            int totalLen = _headSize + _flagSize + GetBodyLengh(Out);
            using var allDataBufferOwner = _memoryPool.Rent(totalLen);
            var allDataBuffer = allDataBufferOwner.Memory[..totalLen];
            PutFlag(allDataBuffer[.._flagSize]);
            PutHeader(allDataBuffer[.._flagSize], allDataBuffer[_flagSize..(_flagSize + _headSize)], Out);
            PutData(allDataBuffer[.._flagSize], allDataBuffer[_flagSize..(_flagSize + _headSize)], allDataBuffer[(_flagSize + _headSize)..], Out);
            return allDataBuffer;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Write(T data)
        {
            if (Out is not null)
            {
                throw new AccessViolationException($"这个包已经被消费.");
            }
            if (data is null || Disposed || TcpClient is null || TcpClient.Disposed)
            {
                return false;
            }
            try
            {
                Out = data;
                return TcpClient.Send(Pack());
            }
            catch (Exception ex)
            {
                _logger?.Error($"向TCP写入消息报错.", exception: ex);
            }
            return false;
        }

        /// <summary>
        /// 尝试读取
        /// </summary>
        /// <param name="ms">等待毫秒数</param> 
        /// <returns></returns>
        public bool TryRead(int ms)
        {
            if (In is not null)
            {
                return true;
            }
            var res = _waitter.WaitOne(ms);

            return res;
        }


        /// <summary>
        /// 检查标志位
        /// </summary>
        /// <param name="flagBuffer">标志位buffer</param>
        /// <returns></returns>
        public abstract bool CheckFlag(in Memory<byte> flagBuffer);


        /// <summary>
        /// 检查头
        /// </summary>
        /// <param name="flagBuffer">标识位缓冲区</param>
        /// <param name="headBuffer">头部缓冲</param>
        /// <param name="bodyLength">内容长度</param>
        /// <param name="headerID">头ID</param>
        /// <returns></returns>
        public abstract bool CheckHeader(in Memory<byte>? flagBuffer, in Memory<byte> headBuffer, out int bodyLength, out Memory<byte> headerID);


        /// <summary>
        /// 检查标志位
        /// </summary>
        /// <param name="flagBuffer">标志位buffer</param>
        /// <returns></returns>
        protected abstract void PutFlag(Memory<byte> flagBuffer);

        /// <summary>
        /// 检查头
        /// </summary>
        /// <param name="flagBuffer">标识位缓冲区</param>
        /// <param name="headBuffer">头部缓冲</param> 
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected abstract void PutHeader(in Memory<byte>? flagBuffer, Memory<byte> headBuffer, in T data);


        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="flagBuffer">标识位buffer</param>
        /// <param name="headBuffer">头部buffer</param>
        /// <param name="bodyBuffer">数据buffer</param>
        /// <param name="data">数据</param>
        protected abstract void PutData(in Memory<byte>? flagBuffer, in Memory<byte> headBuffer, Memory<byte> bodyBuffer, in T data);

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="flagBuffer">标识位buffer</param>
        /// <param name="headBuffer">头部buffer</param>
        /// <param name="bodyBuffer">数据buffer</param>
        public abstract T HandleData(in Memory<byte>? flagBuffer, in Memory<byte> headBuffer, in Memory<byte> bodyBuffer);


        /// <summary>
        /// 获取数据的内容长度
        /// </summary>
        /// <param name="data"></param>
        protected abstract int GetBodyLengh(T data);
    }
}
