namespace Asgard.IDGen
{
    /// <summary>
    /// this IDGen use snowflake
    /// </summary>
    public class SnowflakeIDGen
    {
        private static SnowflakeIDGen? _iDWorker = null;
        private const long _twepoch = 1288834974657L;

        private const int _workerIdBits = 5;

        private const int _datacenterIdBits = 5;
        private const int _sequenceBits = 12;

        private const long _maxWorkerId = -1L ^ -1L << _workerIdBits;

        private const long _maxDatacenterId = -1L ^ -1L << _datacenterIdBits;

        private const int _workerIdShift = _sequenceBits;
        private const int _datacenterIdShift = _sequenceBits + _workerIdBits;

        private const int _timestampLeftShift = _sequenceBits + _workerIdBits + _datacenterIdBits;

        private const long _sequenceMask = -1L ^ -1L << _sequenceBits;

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;

        private SnowflakeIDGen() : this(0, 0)
        {
        }
        private SnowflakeIDGen(long workerId, long datacenterId, long sequence = 0L)
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            _sequence = sequence;
            if (workerId > _maxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id can't be greater than {0} or less than 0", _maxWorkerId));
            }

            if (datacenterId > _maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(string.Format("datacenter Id can't be greater than {0} or less than 0", _maxDatacenterId));
            }
        }

        /// <summary>
        /// set this process's id gen work and datacenter 
        ///  ensure that this method invoked before all the calls 
        ///  otherwise it's not work
        /// </summary>
        public static void Init(long workerID, long datacenter)
        {
            if (_iDWorker == null)
            {
                _iDWorker = new SnowflakeIDGen(workerID, datacenter);
            }
        }


        /// <summary>
        /// IDGenInstance if you not init, it was null ptr
        /// </summary>
        public static SnowflakeIDGen? Instance { get => _iDWorker; }

        private long WorkerId { get; set; }
        private long DatacenterId { get; set; }




        private readonly object _lock = new();
        /// <summary>
        /// get the nex id
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                {
                    throw new Exception(string.Format(
                        "Clock moved backwards.  Refusing to generate id for {0} milliseconds", _lastTimestamp - timestamp));
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & _sequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = timestamp - _twepoch << _timestampLeftShift |
                         DatacenterId << _datacenterIdShift |
                         WorkerId << _workerIdShift | _sequence;

                return id;
            }
        }

        private static long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private static long TimeGen()
        {
            return IdWorkSystem.CurrentTimeMillis();
        }
    }
    internal static class IdWorkSystem
    {
        public static Func<long> currentTimeFunc = InternalCurrentTimeMillis;

        public static long CurrentTimeMillis()
        {
            return currentTimeFunc();
        }

        public static IDisposable StubCurrentTime(Func<long> func)
        {
            currentTimeFunc = func;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        public static IDisposable StubCurrentTime(long millis)
        {
            currentTimeFunc = () => millis;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        private static readonly DateTime _jan1st1970 = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long InternalCurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - _jan1st1970).TotalMilliseconds;
        }
    }
    internal class DisposableAction : IDisposable
    {
        private readonly Action _action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DisposableAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _action();
        }
    }
}
