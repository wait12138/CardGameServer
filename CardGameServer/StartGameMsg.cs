using System;

namespace CardGameServer
{
    class StartGameMsg : BaseMsg
    {
        public bool gameStarted;
        public override byte[] Writing()
        {
            int index = 0;
            int bytesNum = GetBytesNum();
            byte[] bytes = new byte[bytesNum];
            // 写入消息ID
            WriteInt(bytes, GetID(), ref index);
            // 写入消息体的长度
            WriteInt(bytes, bytesNum - 8, ref index);
            // 写入 bool 值
            WriteBool(bytes, gameStarted, ref index);
            return bytes;
        }
        public override int Reading(byte[] bytes, int beginIndex = 0)
        {
            // 反序列化：直接读取 bool 值
            int index = beginIndex;
            gameStarted = ReadBool(bytes, ref index);
            return index - beginIndex;
        }
        public override int GetBytesNum()
        {
            return sizeof(int) + sizeof(bool) + sizeof(int); // 消息ID + 消息体长度 + bool 值的长度
        }
        public override int GetID()
        {
            return 1002; // 定义游戏开始消息的ID为1002
        }
    }
}
