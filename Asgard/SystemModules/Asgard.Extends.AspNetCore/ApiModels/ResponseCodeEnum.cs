using System.ComponentModel;

namespace Asgard.Extends.AspNetCore.ApiModels
{
    /// <summary>
    /// Response Code
    /// </summary>
    [Flags]
    public enum ResponseCodeEnum
    {
        /// <summary>
        /// Success
        /// </summary>
        [Description("Success")]
        Success = 0,
        /// <summary>
        /// Error
        /// </summary>
        [Description("Error")]
        Error = 1,
        /// <summary>
        /// System internal
        /// </summary>
        [Description("System internal")]
        System = 2,
        /// <summary>
        /// Logic error
        /// </summary>
        [Description("Logic error")]
        Logic = 4,
        /// <summary>
        /// Parameter error
        /// </summary>
        [Description("Parameter error")]
        Parameter = 8,
        /// <summary>
        /// Network error
        /// </summary>
        [Description("Network error")]
        Network = 16
    }
}
