using System.Text;

using Asgard.Abstract.Communication.Tcp;
using Asgard.Abstract.Logger;

namespace Asgard.ConfigCenter.TCPPackage
{
    /// <summary>
    /// 配置中心消息包
    /// </summary>
    public class ConfigCenterMessagePackage : AbsTcpPackage<byte[]>
    {
        private static readonly byte[] _flag = new byte[] { 0x55, 0xaa };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="id"></param>
        public ConfigCenterMessagePackage(AbsLogger logger, Memory<byte> id) : base(logger, id)
        {
            _flagSize = 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flagBuffer"></param>
        /// <returns></returns>
        public override bool CheckFlag(in Memory<byte> flagBuffer)
        {
            return flagBuffer.Span.SequenceEqual(_flag);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flagBuffer"></param>
        /// <param name="headBuffer"></param>
        /// <param name="bodyLength"></param>
        /// <param name="headerID"></param>
        /// <returns></returns>
        public override bool CheckHeader(in Memory<byte>? flagBuffer, in Memory<byte> headBuffer, out int bodyLength, out Memory<byte> headerID)
        {
            bodyLength = BitConverter.ToInt32(headBuffer.Span[0..4]);
            headerID = headBuffer[4..].ToArray();
            ID = headerID;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flagBuffer"></param>
        /// <param name="headBuffer"></param>
        /// <param name="bodyBuffer"></param>
        /// <returns></returns>
        public override byte[] HandleData(in Memory<byte>? flagBuffer, in Memory<byte> headBuffer, in Memory<byte> bodyBuffer)
        {
            return bodyBuffer.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override int GetBodyLengh(byte[] data)
        {
            return data.Length;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flagBuffer"></param>
        /// <param name="headBuffer"></param>
        /// <param name="bodyBuffer"></param>
        /// <param name="data"></param>
        protected override void PutData(in Memory<byte>? flagBuffer, in Memory<byte> headBuffer, Memory<byte> bodyBuffer, in byte[] data)
        {
            data.CopyTo(bodyBuffer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flagBuffer"></param>
        protected override void PutFlag(Memory<byte> flagBuffer)
        {
            _flag.CopyTo(flagBuffer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flagBuffer"></param>
        /// <param name="headBuffer"></param>
        /// <param name="data"></param>
        protected override void PutHeader(in Memory<byte>? flagBuffer, Memory<byte> headBuffer, in byte[] data)
        {
            BitConverter.GetBytes(GetBodyLengh(data)).CopyTo(headBuffer);
            ID.CopyTo(headBuffer[4..]);
        }

        /// <summary>
        /// 获取操作类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool TryGetOpType(out ConfigCenterOperationEnum? type)
        {
            type = null;
            if (In is null)
            {
                return false;
            }
            type = (ConfigCenterOperationEnum)BitConverter.ToInt32(In[..4]);
            return true;
        }

        /// <summary>
        /// 尝试获取裸字符串内容
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetStringBody(out string? data)
        {
            data = default;
            if (In is null)
            {
                return false;
            }
            data = Encoding.UTF8.GetString(In[4..]);
            return true;
        }

        /// <summary>
        /// 尝试获取内容对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetBody<T>(out T? data)
        {
            data = default;
            if (In is null)
            {
                return false;
            }
            data = System.Text.Json.JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(In[4..]));
            return true;
        }
    }
}
