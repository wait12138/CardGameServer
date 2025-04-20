using System.Collections;
using System.Collections.Generic;

public class AtkPlayerMsg : BaseMsg
{
    public int clientId; // 发送攻击消息的客户端ID
    public int targetClientId; // 攻击目标的客户端ID
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index); // 先写入消息ID
        WriteInt(bytes, GetBytesNum() - 8, ref index); // 再写入消息体长度
        WriteInt(bytes, clientId, ref index); // 再写入客户端ID
        WriteInt(bytes, targetClientId, ref index); // 再写入目标客户端ID
        return bytes;
    }
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        clientId = ReadInt(bytes, ref index); // 读取客户端ID
        targetClientId = ReadInt(bytes, ref index); // 读取目标客户端ID
        return index - beginIndex; // 返回读取的字节数
    }
    public override int GetBytesNum()
    {
        return 4 + // 消息ID
               4 + // 消息体长度
               4 + // 客户端ID
               4; // 目标客户端ID
    }
    public override int GetID()
    {
        return 1010; // 自定义攻击消息的ID为1010
    }
}