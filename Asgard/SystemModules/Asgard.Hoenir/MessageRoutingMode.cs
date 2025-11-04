namespace Asgard.Hoenir
{
    /// <summary>
    /// Message routing modes for A2A communication
    /// </summary>
    public enum MessageRoutingModeEnum
    {
        /// <summary>
        /// Unicast to specific agent
        /// </summary>
        Unicast,
        
        /// <summary>
        /// Broadcast to all subscribers
        /// </summary>
        Broadcast,
        
        /// <summary>
        /// Multicast to agent group
        /// </summary>
        Group
    }
}