using System.Collections;
using System.Collections.Generic;

public class PlayerDefenceMsg : BaseMsg
{
    public int clientId; // 发送者的客户端ID
    public bool isDefence; // 是否处于防御状态
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index); // 先写入消息ID
        WriteInt(bytes, GetBytesNum() - 8, ref index); // 再写入消息体长度
        WriteInt(bytes, clientId, ref index); // 再写入客户端ID
        WriteBool(bytes, isDefence, ref index); // 再写入是否处于防御状态
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        clientId = ReadInt(bytes, ref index); // 读取客户端ID
        isDefence = ReadBool(bytes, ref index); // 读取是否处于防御状态
        return index - beginIndex; // 返回读取的字节数
    }
    public override int GetBytesNum()
    {
        return sizeof(int) + // 消息ID
               sizeof(int) + // 消息体长度
               sizeof(int) + // 客户端ID
               sizeof(bool); // 是否处于防御状态
    }
    public override int GetID()
    {
        return 1008; // 自定义防御消息的ID为1008
    }
}
