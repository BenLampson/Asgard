namespace Asgard.Hoenir
{
    /// <summary>
    /// Message payload for event bus and A2A communication
    /// </summary>
    public class MessageDataItem
    {
        /// <summary>
        /// Constructs a new message
        /// </summary>
        /// <param name="source">Message source for deduplication filtering</param>
        public MessageDataItem(string? source = null)
        {
            Source = source ?? "";
        }

        /// <summary>
        /// Message originator identifier
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Unique message identifier
        /// </summary>
        public string MessageID { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Message payload data
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Message creation timestamp
        /// </summary>
        public long TimeStamp { get; init; } = DateTime.Now.Ticks;

        /// <summary>
        /// Source file path for debugging
        /// </summary>
        public string FromFile { get; set; } = string.Empty;

        /// <summary>
        /// Source line number for debugging
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Custom headers for routing and metadata
        /// </summary>
        public Dictionary<string, object> Header { get; set; } = new();

        /// <summary>
        /// Target agent ID for unicast routing. Null for broadcast.
        /// </summary>
        public string? TargetAgentId { get; set; }

        /// <summary>
        /// Source agent ID for reply routing
        /// </summary>
        public string? SourceAgentId { get; set; }

        /// <summary>
        /// Message routing mode for A2A communication
        /// </summary>
        public MessageRoutingModeEnum RoutingMode { get; set; } = MessageRoutingModeEnum.Broadcast;

        /// <summary>
        /// Gets typed data if compatible
        /// </summary>
        /// <typeparam name="T">Target type for conversion</typeparam>
        /// <returns>Typed data or default</returns>
        public T? GetData<T>()
        {
            if (Data is T res)
            {
                return res;
            }
            return default;
        }

        /// <summary>
        /// Attempts to get typed data
        /// </summary>
        /// <typeparam name="T">Target type for conversion</typeparam>
        /// <returns>True if conversion successful</returns>
        public bool TryGetData<T>(out T? data)
        {
            data = default;
            if (Data is T res)
            {
                data = res;
                return true;
            }
            return false;
        }
    }
}
