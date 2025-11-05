namespace Asgard.Extends.AspNetCore.ApiModels
{
    /// <summary>
    /// Response base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataResponse<T>
    {
        /// <summary>
        /// Response status
        /// </summary>
        public ResponseCodeEnum Code { get; set; }
        /// <summary>
        /// Custom message
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// Carried data
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Total data count
        /// </summary>
        public int DataCount { get; set; }
    }
}
