using System;
using System.Text;

namespace CardGameServer
{
    class ClientIdMsg : BaseMsg
    {
        public int clientId;
        public override byte[] Writing()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesNum()];
            //先写入消息ID
            //再写入消息体长度
            //再写入clientId
            WriteInt(bytes, GetID(), ref index);
            WriteInt(bytes, GetBytesNum() - 8, ref index);
            WriteInt(bytes, clientId, ref index);
            return bytes;
        }
        public override int Reading(byte[] bytes, int beginIndex = 0)
        {
            //反序列化不需要知道消息ID 在这一步之前 就应该把消息ID解析出来了
            //用来判断用哪一个自定义类来解析
            int index = beginIndex;
            clientId = ReadInt(bytes, ref index);
            return index - beginIndex;
        }
        public override int GetBytesNum()
        {
            return sizeof(int) + sizeof(int) + sizeof(int); // 消息ID + 消息体长度 + clientId的长度
        }
        public override int GetID()
        {
            return 1001;//自定义客户端ID消息的ID为1001
        }
    }
}
